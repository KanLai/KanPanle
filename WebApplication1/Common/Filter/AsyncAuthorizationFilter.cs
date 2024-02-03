using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using WebApplication1.Attrs;

namespace WebApplication1;

public class AsyncAuthorizationFilter(ApiConfiguration configuration) : IAsyncAuthorizationFilter
{
    public Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        try
        {
            // 读取token
            var tokenString = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();
            var dbContext = context.HttpContext.RequestServices.GetService<ApplicationDbContext>();
            if (!string.IsNullOrEmpty(tokenString) &&
                SecurityTokenValid(tokenString, out var token))
            {
                if (token is null)
                {
                    context.Result = new UnauthorizedObjectResult(new Json<string>()
                    {
                        code = 0,
                        message = "token 无效",
                        data = null,
                        time = DateTimeOffset.Now.ToUnixTimeSeconds()
                    });
                    return Task.CompletedTask;
                }

                var payload = JsonConvert.DeserializeObject<Dictionary<string, string>>(
                    Base64UrlEncoder.Decode(token.EncodedPayload));


                //转换成int类型
                var userId = long.Parse(payload!["Id"]);
                var userData = dbContext?.Users.Find(userId);
                if (userData is null)
                {
                    context.Result = new UnauthorizedObjectResult(new Json<string>()
                    {
                        code = 0,
                        message = "用户不存在",
                        data = null,
                        time = DateTimeOffset.Now.ToUnixTimeSeconds()
                    });
                    return Task.CompletedTask;
                }

                context.HttpContext.Items.Add("User", userData);

                if (userData.Token == tokenString) return Task.CompletedTask;
                context.Result = new UnauthorizedObjectResult(new Json<string>()
                {
                    code = 0,
                    message = "Token检查失败",
                    data = null,
                    time = DateTimeOffset.Now.ToUnixTimeSeconds()
                });
                return Task.CompletedTask;
            }

            // 读取自定义属性
            var messageAttribute = context.ActionDescriptor.EndpointMetadata
                .OfType<AuthorizationMessageAttribute>()
                .FirstOrDefault();

            var message = messageAttribute?.Message ?? "未登录"; // 使用自定义消息，如果没有则使用默认值

            context.Result = new UnauthorizedObjectResult(new Json<string>()
            {
                code = 0,
                message = message,
                data = null,
                time = DateTimeOffset.Now.ToUnixTimeSeconds()
            });
            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    private bool SecurityTokenValid(string jwtToken, out JwtSecurityToken? token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(jwtToken, configuration.GetTokenValidationParameters(),
                out var securityToken);
            token = securityToken as JwtSecurityToken;
            return true;
        }
        catch (Exception)
        {
            token = new JwtSecurityToken("");
            return false;
        }
    }
}