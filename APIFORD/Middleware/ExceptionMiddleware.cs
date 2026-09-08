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
            ServiceUnavailableException => (int)HttpStatusCode.ServiceUnavailable, // 503
            BadRequestException => (int)HttpStatusCode.BadRequest, // 400
            UnauthorizedException => (int)HttpStatusCode.Unauthorized, // 401
            ForbiddenException => (int)HttpStatusCode.Forbidden, // 403
            NotFoundException => (int)HttpStatusCode.NotFound, // 404
            ConflictException => (int)HttpStatusCode.Conflict, // 409
            ValidationException => (int)HttpStatusCode.UnprocessableEntity, // 422
            ExternalServiceException => (int)HttpStatusCode.BadGateway, // 502
            _ => (int)HttpStatusCode.InternalServerError
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