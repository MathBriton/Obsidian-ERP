using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ObsidianERP.Api.ErrorHandling;

/// <summary>
/// Captura qualquer exceção não tratada e devolve uma resposta padronizada RFC 7807
/// (ProblemDetails / application/problem+json), registrando o erro de forma estruturada.
/// </summary>
public sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title) = ProblemDetailsMapping.Map(exception);

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            logger.LogError(
                exception,
                "Erro não tratado em {Method} {Path}",
                httpContext.Request.Method,
                httpContext.Request.Path);
        }
        else
        {
            logger.LogWarning(
                "Requisição rejeitada ({StatusCode}) em {Method} {Path}: {Message}",
                statusCode,
                httpContext.Request.Method,
                httpContext.Request.Path,
                exception.Message);
        }

        httpContext.Response.StatusCode = statusCode;

        // Mensagens de negócio (4xx) são seguras de expor; erros 5xx não vazam detalhes.
        var detail = statusCode >= StatusCodes.Status500InternalServerError
            ? "Ocorreu um erro inesperado. Tente novamente mais tarde."
            : exception.Message;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
            },
        });
    }
}
