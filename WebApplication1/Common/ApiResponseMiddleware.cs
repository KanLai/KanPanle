using Newtonsoft.Json;
using WebApplication1;

public class ApiResponseMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            context.Response.Headers["Server"] = "www.kanlai.com.cn";
            context.Response.Headers["Author"] = "admin@kanlai.com.cn";
            context.Response.Headers["qq"] = "380943047";
            return Task.CompletedTask;
        });

        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var response = new Json<Exception>()
        {
            code = 500,
            message = ex.Message,
            data = ex
        };

        await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
    }
}