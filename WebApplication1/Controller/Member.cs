using Microsoft.AspNetCore.Mvc;
using WebApplication1.Attrs;
using WebApplication1.Dto;
using WebApplication1.Model;

namespace WebApplication1.Controller;

public class UserTo
{
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? Name { get; set; }
    public string? Avatar { get; set; }
    public long Id { get; set; }
}

[AuthorizationMessage("请先登录")]
[ServiceFilter(typeof(AsyncAuthorizationFilter))]
[ApiController]
[Route("[controller]")]
public class Member(ApplicationDbContext db, ApiConfiguration apiConfiguration) : ControllerBase
{
    [HttpPut("update")]
    public IActionResult update([FromBody] UserTo user, CachedConfigService configuration)
    {
        var oldUser = db.Users.Find(user.Id);
        if (oldUser is null)
        {
            return NotFound(new Json<object>()
            {
                code = 0,
                message = "更新失败",
                data = null,
                time = DateTimeOffset.Now.ToUnixTimeSeconds()
            });
        }

        oldUser.UserName = !string.IsNullOrEmpty(user.UserName) ? user.UserName : oldUser.UserName;

        oldUser.Password = !string.IsNullOrEmpty(user.Password) ? user.Password : oldUser.Password;

        var U = new User
        {
            UserName = oldUser.UserName,
            Password = oldUser.Password,
            Name = user.Name!,
            Avatar = user.Avatar!,
            Id = user.Id,
        };

        var token = apiConfiguration.GenerateToken(U, out _);

        if (user.UserName != oldUser.UserName)
        {
            var userExist = db.Users.SingleOrDefault(u => u.UserName == user.UserName && u.Id != user.Id);
            if (userExist != null)
            {
                return BadRequest(new Json<object>()
                {
                    code = 0,
                    message = "用户名已存在",
                    data = null,
                    time = DateTimeOffset.Now.ToUnixTimeSeconds()
                });
            }
        }

        //用户名和密码不为空就更新

        oldUser.Token = token;
        oldUser.UpdateTime = DateTime.Now;
        oldUser.Name = user.Name!;
        oldUser.Avatar = user.Avatar!;
        db.SaveChanges();
        return Ok(new Json<UserDto>()
        {
            code = 1,
            message = "更新成功",
            data = UserDto.From(oldUser, configuration),
            time = DateTimeOffset.Now.ToUnixTimeSeconds()
        });
    }
}