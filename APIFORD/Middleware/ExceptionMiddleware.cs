using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace APIFORD.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        // Mapeia o tipo de exceção do C# para o Status Code correto do HTTP
        context.Response.StatusCode = exception switch
        {
            ApplicationException => (int)HttpStatusCode.BadRequest, // Erro de validação/negócio
            KeyNotFoundException => (int)HttpStatusCode.NotFound,   // Registro não existe
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized, // Sem autorização
            _ => (int)HttpStatusCode.InternalServerError // Erro interno do servidor (500)
        };

        var response = new
        {
            status = context.Response.StatusCode,
            message = exception.Message,
            // Detalhes extras se houverem (ex: erro do banco de dados)
            detail = exception.InnerException?.Message
        };

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}