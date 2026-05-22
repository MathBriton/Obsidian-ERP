using FluentAssertions;
using NSubstitute;
using ObsidianERP.Application.Abstractions;
using ObsidianERP.Application.Common;
using ObsidianERP.Application.DTOs;
using ObsidianERP.Application.Exceptions;
using ObsidianERP.Application.Services;
using ObsidianERP.Domain.Entities;

namespace ObsidianERP.Tests.Auth;

public class AuthServiceTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly IRefreshTokenRepository _refreshTokens = Substitute.For<IRefreshTokenRepository>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
    private readonly IJwtTokenGenerator _tokens = Substitute.For<IJwtTokenGenerator>();

    private readonly JwtSettings _settings = new()
    {
        Issuer = "i",
        Audience = "a",
        Secret = "segredo-de-teste-com-pelo-menos-32-caracteres!!",
        AccessTokenMinutes = 15,
        RefreshTokenDays = 7,
    };

    private AuthService CreateSut() => new(_users, _refreshTokens, _hasher, _tokens, _settings);

    [Fact]
    public async Task RegisterAsync_com_email_novo_cria_usuario_e_retorna_tokens()
    {
        _users.EmailExistsAsync("ana@x.com", Arg.Any<CancellationToken>()).Returns(false);
        _hasher.Hash("senha123").Returns("hash");
        _tokens.GenerateAccessToken(Arg.Any<User>())
            .Returns(new AccessToken("access-token", DateTime.UtcNow.AddMinutes(15)));
        _tokens.GenerateRefreshToken().Returns("refresh-token");

        var result = await CreateSut().RegisterAsync(new RegisterRequest("Ana", "ana@x.com", "senha123"));

        result.AccessToken.Should().Be("access-token");
        result.RefreshToken.Should().Be("refresh-token");
        result.User.Email.Should().Be("ana@x.com");
        await _users.Received(1).AddAsync(
            Arg.Is<User>(u => u.Email == "ana@x.com" && u.PasswordHash == "hash"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RegisterAsync_com_email_existente_lanca_EmailAlreadyInUse()
    {
        _users.EmailExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(true);

        await CreateSut()
            .Invoking(s => s.RegisterAsync(new RegisterRequest("Ana", "ana@x.com", "senha123")))
            .Should().ThrowAsync<EmailAlreadyInUseException>();
    }

    [Fact]
    public async Task LoginAsync_com_credenciais_validas_retorna_tokens()
    {
        var user = new User { Name = "Ana", Email = "ana@x.com", PasswordHash = "hash" };
        _users.GetByEmailAsync("ana@x.com", Arg.Any<CancellationToken>()).Returns(user);
        _hasher.Verify("senha123", "hash").Returns(true);
        _tokens.GenerateAccessToken(user).Returns(new AccessToken("access", DateTime.UtcNow.AddMinutes(15)));
        _tokens.GenerateRefreshToken().Returns("refresh");

        var result = await CreateSut().LoginAsync(new LoginRequest("ana@x.com", "senha123"));

        result.AccessToken.Should().Be("access");
        result.User.Email.Should().Be("ana@x.com");
    }

    [Fact]
    public async Task LoginAsync_com_senha_incorreta_lanca_InvalidCredentials()
    {
        var user = new User { Name = "Ana", Email = "ana@x.com", PasswordHash = "hash" };
        _users.GetByEmailAsync("ana@x.com", Arg.Any<CancellationToken>()).Returns(user);
        _hasher.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

        await CreateSut()
            .Invoking(s => s.LoginAsync(new LoginRequest("ana@x.com", "errada")))
            .Should().ThrowAsync<InvalidCredentialsException>();
    }

    [Fact]
    public async Task LoginAsync_com_email_inexistente_lanca_InvalidCredentials()
    {
        _users.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((User?)null);

        await CreateSut()
            .Invoking(s => s.LoginAsync(new LoginRequest("ninguem@x.com", "senha123")))
            .Should().ThrowAsync<InvalidCredentialsException>();
    }

    [Fact]
    public async Task RefreshAsync_com_token_invalido_lanca_InvalidRefreshToken()
    {
        _refreshTokens.GetByTokenAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((RefreshToken?)null);

        await CreateSut()
            .Invoking(s => s.RefreshAsync("inexistente"))
            .Should().ThrowAsync<InvalidRefreshTokenException>();
    }

    [Fact]
    public async Task RefreshAsync_com_token_valido_revoga_o_antigo_e_retorna_novos_tokens()
    {
        var user = new User { Name = "Ana", Email = "ana@x.com", PasswordHash = "hash" };
        var stored = new RefreshToken
        {
            Token = "valido",
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            User = user,
        };
        _refreshTokens.GetByTokenAsync("valido", Arg.Any<CancellationToken>()).Returns(stored);
        _tokens.GenerateAccessToken(user).Returns(new AccessToken("novo-access", DateTime.UtcNow.AddMinutes(15)));
        _tokens.GenerateRefreshToken().Returns("novo-refresh");

        var result = await CreateSut().RefreshAsync("valido");

        result.AccessToken.Should().Be("novo-access");
        result.RefreshToken.Should().Be("novo-refresh");
        stored.RevokedAt.Should().NotBeNull();
    }
}
