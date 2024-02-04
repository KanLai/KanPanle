using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Attrs;
using WebApplication1.Dto;
using WebApplication1.Model;

namespace WebApplication1.Controller;

[ApiController]
[Route("[controller]")]
public class Home(ApplicationDbContext db, CachedConfigService cachedConfigService) : ControllerBase
{
    [HttpGet("get")]
    public IActionResult get()
    {
        var list = db.Cases.Include(e => e.menus).Select(c => new
        {
            c.id,
            c.title,
            menus = MenuDto.From(c.menus!.OrderBy(e => e.ord).ToList(), cachedConfigService)
        });


        return Ok(new Json<IQueryable>
        {
            code = 1,
            message = "获取成功",
            data = list,
            time = DateTimeOffset.Now.ToUnixTimeSeconds()
        });
    }

    [AuthorizationMessage("请先登录")]
    [ServiceFilter(typeof(AsyncAuthorizationFilter))]
    [HttpPut("add")]
    public IActionResult add([FromBody] Menu menu)
    {
        menu.id = null;
        db.Menus.Add(menu);
        db.SaveChanges();
        return Ok(new Json<MenuDto>()
        {
            code = 1,
            message = "添加成功",
            data = MenuDto.From(menu, cachedConfigService),
            time = DateTimeOffset.Now.ToUnixTimeSeconds()
        });
    }

    [AuthorizationMessage("请先登录")]
    [ServiceFilter(typeof(AsyncAuthorizationFilter))]
    [HttpPut("update")]
    public IActionResult update([FromBody] Menu menu)
    {
        var old = db.Menus.SingleOrDefault(e => e.id == menu.id);
        if (old == null)
            return NotFound(new Json<string?>()
            {
                code = 0,
                message = "数据不存在",
            });
        db.Entry(old).CurrentValues.SetValues(menu);
        db.SaveChanges();
        return Ok(new Json<MenuDto>()
        {
            code = 1,
            message = "更新成功",
            data = MenuDto.From(old, cachedConfigService)
        });
    }

    [AuthorizationMessage("请先登录")]
    [ServiceFilter(typeof(AsyncAuthorizationFilter))]
    [HttpDelete("remove/{id:int}")]
    public IActionResult remove([Required] int id)
    {
        var menu = db.Menus.SingleOrDefault(e => e.id == id);
        if (menu == null)
            return NotFound(new Json<string?>()
            {
                code = 0,
                message = "删除失败",
            });
        db.Menus.Remove(menu);
        db.SaveChanges();
        return Ok(new Json<string?>()
        {
            code = 1,
            message = "删除成功",
        });
    }

    [AuthorizationMessage("请先登录")]
    [ServiceFilter(typeof(AsyncAuthorizationFilter))]
    [HttpPut("updateOrder")]
    public async Task<IActionResult> UpdateOrder([FromBody] List<OrdDto> ids)
    {
        await using (var transaction = await db.Database.BeginTransactionAsync())
        {
            try
            {
                foreach (var item in ids)
                {
                    var menu = await db.Menus.SingleOrDefaultAsync(e => e.id == item.id);
                    menu!.ord = (int)item.ord!;
                }

                await db.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok(new
                {
                    code = 1,
                    message = "更新成功",
                });
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return BadRequest(new
                {
                    code = 0,
                    message = "更新失败: " + e.Message, // 提供错误信息有助于调试
                });
            }
        }
    }

    [AllowAnonymous]
    [HttpGet("getBg")]
    public IActionResult getBg()
    {
        var url = cachedConfigService.Get<string>("background");
        var containsHttp = url.Contains("http://");
        var containsHttps = url.Contains("https://");
        if (containsHttp || containsHttps)
        {
            return Ok(new Json<string>()
            {
                code = 1,
                message = "获取成功",
                data = url,
            });
        }

        return Ok(new Json<string>()
        {
            code = 1,
            message = "获取成功",
            data = cachedConfigService.Get<string>("baseUrl") + url,
        });
    }
}