using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace APIFORD.Filters;

public class InternalApiKeyAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var config = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var chaveEsperada = config["PythonInternalApiKey"];

        if (!context.HttpContext.Request.Headers.TryGetValue("X-Internal-Api-Key", out var chaveRecebida) ||
            chaveRecebida != chaveEsperada)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        await next();
    }
}
