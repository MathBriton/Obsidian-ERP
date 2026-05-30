using FluentAssertions;
using FluentValidation;
using ObsidianERP.Api.ErrorHandling;
using ObsidianERP.Application.Exceptions;
using ObsidianERP.Domain.Exceptions;

namespace ObsidianERP.Tests.Quality;

public class ProblemDetailsMappingTests
{
    [Fact]
    public void Recurso_nao_encontrado_mapeia_para_404()
    {
        ProblemDetailsMapping.Map(new CustomerNotFoundException(Guid.NewGuid()))
            .StatusCode.Should().Be(404);
        ProblemDetailsMapping.Map(new OrderNotFoundException(Guid.NewGuid()))
            .StatusCode.Should().Be(404);
    }

    [Fact]
    public void Email_em_uso_mapeia_para_409()
    {
        ProblemDetailsMapping.Map(new EmailAlreadyInUseException("a@b.com"))
            .StatusCode.Should().Be(409);
    }

    [Fact]
    public void Credenciais_e_refresh_invalidos_mapeiam_para_401()
    {
        ProblemDetailsMapping.Map(new InvalidCredentialsException()).StatusCode.Should().Be(401);
        ProblemDetailsMapping.Map(new InvalidRefreshTokenException()).StatusCode.Should().Be(401);
    }

    [Fact]
    public void Regras_de_dominio_mapeiam_para_409()
    {
        ProblemDetailsMapping.Map(new EmptyOrderException()).StatusCode.Should().Be(409);
        ProblemDetailsMapping.Map(new OrderCannotBeModifiedException()).StatusCode.Should().Be(409);
        ProblemDetailsMapping.Map(new OrderAlreadyCancelledException()).StatusCode.Should().Be(409);
    }

    [Fact]
    public void Validacao_mapeia_para_400()
    {
        ProblemDetailsMapping.Map(new ValidationException("inválido")).StatusCode.Should().Be(400);
    }

    [Fact]
    public void Excecao_desconhecida_mapeia_para_500()
    {
        var (status, title) = ProblemDetailsMapping.Map(new InvalidOperationException("boom"));
        status.Should().Be(500);
        title.Should().NotBeNullOrWhiteSpace();
    }
}
