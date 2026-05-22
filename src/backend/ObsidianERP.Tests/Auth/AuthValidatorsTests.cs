using FluentAssertions;
using FluentValidation.TestHelper;
using ObsidianERP.Application.DTOs;
using ObsidianERP.Application.Validators;

namespace ObsidianERP.Tests.Auth;

public class AuthValidatorsTests
{
    private readonly RegisterRequestValidator _registerValidator = new();
    private readonly LoginRequestValidator _loginValidator = new();

    [Fact]
    public void Register_valido_passa()
    {
        var result = _registerValidator.TestValidate(new RegisterRequest("Ana", "ana@x.com", "senha123"));
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("nao-e-email")]
    public void Register_com_email_invalido_falha(string email)
    {
        var result = _registerValidator.TestValidate(new RegisterRequest("Ana", email, "senha123"));
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Register_com_senha_curta_falha()
    {
        var result = _registerValidator.TestValidate(new RegisterRequest("Ana", "ana@x.com", "123"));
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Register_com_nome_vazio_falha()
    {
        var result = _registerValidator.TestValidate(new RegisterRequest("", "ana@x.com", "senha123"));
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Login_valido_passa()
    {
        var result = _loginValidator.TestValidate(new LoginRequest("ana@x.com", "senha123"));
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Login_com_email_vazio_falha()
    {
        var result = _loginValidator.TestValidate(new LoginRequest("", "senha123"));
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }
}
