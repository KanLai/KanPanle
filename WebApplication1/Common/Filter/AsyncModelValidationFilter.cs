using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApplication1;

public class AsyncModelValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(kvp => kvp.Value is { Errors.Count: > 0 })
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            var errorResponse = new Json<Dictionary<string, string[]?>>()
            {
                code = 0,
                message = errors.First().Value?.First() ?? "参数错误",
                data = errors,
                time = DateTimeOffset.Now.ToUnixTimeSeconds()
            };
            context.Result = new BadRequestObjectResult(errorResponse);
            return;
        }
        await next();
    }
}