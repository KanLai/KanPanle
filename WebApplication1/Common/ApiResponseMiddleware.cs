using Newtonsoft.Json;
using WebApplication1;

public class ApiResponseMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
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