
namespace APIFORD.Middleware;


public abstract class ApiException : Exception
{
    public int StatusCode { get; }
    protected ApiException(int statusCode, string message) : base(message) { }
}


public class BadRequestException : ApiException
{
    /// <summary>
    /// 400 - request malformado ou combinação de parâmetros inválida
    /// </summary>
    public BadRequestException(string message) : base(StatusCodes.Status400BadRequest, message) { }
}

public class UnauthorizedException : ApiException
{
    /// <summary>
    /// 401 - identidade não comprovada (raro precisar disparar manualmente, [Authorize] já cobre a maioria)
    /// </summary>
    public UnauthorizedException(string message) : base(StatusCodes.Status401Unauthorized, message) { }
}


public class ForbiddenException : ApiException
{
    /// <summary>
    /// 403 - identidade OK, mas sem permissão pra essa ação específica
    /// </summary>
    public ForbiddenException(string message) : base(StatusCodes.Status403Forbidden, message) { }
}


public class NotFoundException : ApiException
{
    /// <summary>
    /// 404 - recurso não existe
    /// </summary>
    public NotFoundException(string message) : base(StatusCodes.Status404NotFound, message) { }
}


public class ConflictException : ApiException
{
    /// <summary>
    /// 409 - estado atual do recurso conflita com a operação pedida
    /// </summary>
    public ConflictException(string message) : base(StatusCodes.Status409Conflict, message) { }
}


public class ValidationException : ApiException
{
    public IDictionary<string, string[]> Errors { get; }
    /// <summary>
    /// 422 - sintaticamente válido, mas semanticamente errado (validação de negócio, não de tipo)
    /// </summary>
    public ValidationException(IDictionary<string, string[]> errors)
        : base(StatusCodes.Status422UnprocessableEntity, "Um ou mais campos são inválidos.")
    {
        Errors = errors;
    }
    /// <summary>
    /// 422 - sintaticamente válido, mas semanticamente errado (validação de negócio, não de tipo)
    /// </summary>
    public ValidationException(string campo, string erro)
        : this(new Dictionary<string, string[]> { [campo] = new[] { erro } }) { }
}

public class InternalServerErrorException : ApiException
{
    /// <summary>
    /// 500 - erro interno do servidor (não deveria ser disparado manualmente, mas sim capturado pelo middleware)
    /// </summary>
    public InternalServerErrorException(string message) : base(StatusCodes.Status500InternalServerError, message) { }
}

public class NotImplementedApiException : ApiException
{
    /// <summary>
    /// 501 - recurso não implementado
    /// </summary>
    public NotImplementedApiException(string message) : base(StatusCodes.Status501NotImplemented, message) { }
}


public class ExternalServiceException : ApiException
{
    /// <summary>
    /// 502 - uma dependência externa falhou ou respondeu algo inesperado
    /// </summary>
    public ExternalServiceException(string servico, string message)
        : base(StatusCodes.Status502BadGateway, $"Falha ao comunicar com {servico}: {message}") { }
}


public class ServiceUnavailableException : ApiException
{
    /// <summary>
    /// 503 - dependência externa fora do ar / timeout
    /// </summary>  
    public ServiceUnavailableException(string message) : base(StatusCodes.Status503ServiceUnavailable, message) { }
}
