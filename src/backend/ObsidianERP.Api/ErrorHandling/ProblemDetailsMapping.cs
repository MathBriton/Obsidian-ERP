using FluentValidation;
using ObsidianERP.Application.Exceptions;
using ObsidianERP.Domain.Exceptions;

namespace ObsidianERP.Api.ErrorHandling;

/// <summary>
/// Traduz uma exceção do domínio/aplicação no status HTTP e título correspondentes,
/// concentrando em um único lugar a política de "qual erro vira qual resposta".
/// </summary>
public static class ProblemDetailsMapping
{
    public static (int StatusCode, string Title) Map(Exception exception) => exception switch
    {
        ValidationException => (StatusCodes.Status400BadRequest, "Erro de validação"),
        NotFoundException => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
        EmailAlreadyInUseException => (StatusCodes.Status409Conflict, "Conflito"),
        InvalidCredentialsException or InvalidRefreshTokenException
            => (StatusCodes.Status401Unauthorized, "Não autorizado"),
        DomainException => (StatusCodes.Status409Conflict, "Operação inválida"),
        _ => (StatusCodes.Status500InternalServerError, "Erro interno do servidor"),
    };
}
