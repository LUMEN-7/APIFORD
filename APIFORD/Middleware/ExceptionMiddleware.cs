using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace APIFORD.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex, _logger);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger logger)
    {
        context.Response.ContentType = "application/json";

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
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            BadHttpRequestException => (int)HttpStatusCode.BadRequest,
            _ => (int)HttpStatusCode.InternalServerError
        };

        if (context.Response.StatusCode >= StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Erro não tratado ao processar {Method} {Path}", context.Request.Method, context.Request.Path);
        else
            logger.LogWarning(exception, "Requisição rejeitada em {Method} {Path}", context.Request.Method, context.Request.Path);

        var response = new
        {
            status = context.Response.StatusCode,
            message = context.Response.StatusCode >= StatusCodes.Status500InternalServerError
                ? "Ocorreu um erro interno. Tente novamente mais tarde."
                : exception.Message
        };

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}
