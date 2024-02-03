using Microsoft.AspNetCore.Mvc;
using WebApplication1.Attrs;
using WebApplication1.Model;

namespace WebApplication1.Controller;

[ApiController]
[Route("[controller]")]
[AuthorizationMessage("请先登录")]
[ServiceFilter(typeof(AsyncAuthorizationFilter))]
public class Case(ApplicationDbContext db) : ControllerBase
{
    [HttpPost("add")]
    public IActionResult add([FromBody] Model.Case @case)
    {
        @case.id = null;

        db.Cases.Add(@case);
        db.SaveChanges();
        @case.menus = new List<Menu>();
        return Ok(new Json<Model.Case>()
        {
            code = 1,
            message = "添加成功",
            data = @case,
            time = DateTimeOffset.Now.ToUnixTimeSeconds()
        });
    }

    [HttpDelete("delete/{id:int}")]
    public IActionResult delete(int id)
    {
        var @case = db.Cases.Find(id);
        if (@case is null)
        {
            return NotFound(new Json<object>()
            {
                code = 0,
                message = "删除失败",
                data = null,
                time = DateTimeOffset.Now.ToUnixTimeSeconds()
            });
        }

        db.Cases.Remove(@case);
        db.Menus.RemoveRange(db.Menus.Where(e => e.belong == @case.id));
        db.SaveChanges();
        return Ok(new Json<object>()
        {
            code = 1,
            message = "删除成功",
            data = null,
            time = DateTimeOffset.Now.ToUnixTimeSeconds()
        });
    }
    [HttpPut("update")]
    public IActionResult update([FromBody] Model.Case @case)
    {
        var oldCase = db.Cases.Find(@case.id);
        if (oldCase is null)
        {
            return NotFound(new Json<object>()
            {
                code = 0,
                message = "更新失败",
                data = null,
                time = DateTimeOffset.Now.ToUnixTimeSeconds()
            });
        }

        oldCase.title = @case.title;
        oldCase.isDing = @case.isDing;
        db.SaveChanges();
        return Ok(new Json<object>()
        {
            code = 1,
            message = "更新成功",
            data = oldCase,
            time = DateTimeOffset.Now.ToUnixTimeSeconds()
        });
    }
}