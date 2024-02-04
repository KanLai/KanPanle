using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Attrs;
using WebApplication1.Dto;
using WebApplication1.Model;


namespace WebApplication1.Controller;

[ApiController]
[Route("[controller]")]
public class Auth(
    ApplicationDbContext applicationDbContext,
    CachedConfigService configService,
    ApiConfiguration apiConfiguration) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        var UserName = loginRequest.Username;
        var Password = loginRequest.Password;

        var userData = await applicationDbContext.Users.SingleOrDefaultAsync(u => u.UserName == UserName);

        if (userData is null)
        {
            return BadRequest(new Json<object>()
            {
                code = 0,
                message = "用户不存在",
                data = null,
                time = DateTimeOffset.Now.ToUnixTimeSeconds()
            });
        }

        if (Password != userData.Password)
        {
            return BadRequest(new Json<object>()
            {
                code = 0,
                message = "密码错误",
                data = null,
                time = DateTimeOffset.Now.ToUnixTimeSeconds()
            });
        }

        userData.Token = apiConfiguration.GenerateToken(userData, out _);
        userData.LastLoginTime = DateTime.Now;
        userData.LastLoginIp = HttpContext.Connection.RemoteIpAddress!.ToString();
        applicationDbContext.Users.Update(userData);
        await applicationDbContext.SaveChangesAsync();
        return Ok(new Json<UserDto>()
        {
            code = 1,
            message = "登录成功",
            data = UserDto.From(userData, configService)
        });
    }

    [HttpGet("sign")]
    [AuthorizationMessage("请先登录")]
    [ServiceFilter(typeof(AsyncAuthorizationFilter))]
    public IActionResult Sign(CachedConfigService cachedConfigService)
    {
        HttpContext.Items.TryGetValue("User", out var user);
        if (user is not User userData)
        {
            return new UnauthorizedResult();
        }

        var url = cachedConfigService.Get<string>("background");
        var containsHttp = url.Contains("http://");
        var containsHttps = url.Contains("https://");
        if (!containsHttps && !containsHttp)
        {
            url = cachedConfigService.Get<string>("baseUrl") + url;
        }

        return Ok(new Json<object>()
        {
            code = 1,
            message = "登录成功",
            data = new
            {
                bg = url,
                member = UserDto.From(userData, configService)
            },
        });
    }

    [HttpGet("config")]
    [AuthorizationMessage("请先登录")]
    [ServiceFilter(typeof(AsyncAuthorizationFilter))]
    public IActionResult Config()
    {
        var list = applicationDbContext.Configs.ToList();
        return Ok(new Json<List<Config>>()
        {
            code = 1,
            message = "获取成功",
            data = list
        });
    }

    [HttpPut("updateConfig")]
    [AuthorizationMessage("请先登录")]
    [ServiceFilter(typeof(AsyncAuthorizationFilter))]
    public IActionResult UpdateConfig([FromBody] Config config)
    {
        applicationDbContext.Configs.SingleOrDefault(e => e.key == config.key)!.val = config.val;
        applicationDbContext.SaveChanges();
        configService.Set(config.key, config.val);
        return Ok(new Json<object>()
        {
            code = 1,
            message = "登录成功",
            data = null
        });
    }
}