using System.IdentityModel.Tokens.Jwt;
using FluentAssertions;
using ObsidianERP.Application.Common;
using ObsidianERP.Domain.Entities;
using ObsidianERP.Infrastructure.Security;

namespace ObsidianERP.Tests.Auth;

public class JwtTokenGeneratorTests
{
    private readonly JwtSettings _settings = new()
    {
        Issuer = "obsidian-erp",
        Audience = "obsidian-erp-client",
        Secret = "segredo-de-teste-com-pelo-menos-32-caracteres!!",
        AccessTokenMinutes = 15,
        RefreshTokenDays = 7,
    };

    [Fact]
    public void GenerateAccessToken_inclui_claims_do_usuario_e_expira_no_futuro()
    {
        var sut = new JwtTokenGenerator(_settings);
        var user = new User { Name = "Ana", Email = "ana@x.com", PasswordHash = "h" };

        var token = sut.GenerateAccessToken(user);

        token.Value.Should().NotBeNullOrWhiteSpace();
        token.ExpiresAt.Should().BeAfter(DateTime.UtcNow);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token.Value);
        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == user.Id.ToString());
        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == "ana@x.com");
    }

    [Fact]
    public void GenerateRefreshToken_gera_valores_unicos_e_nao_vazios()
    {
        var sut = new JwtTokenGenerator(_settings);

        var first = sut.GenerateRefreshToken();
        var second = sut.GenerateRefreshToken();

        first.Should().NotBeNullOrWhiteSpace();
        first.Should().NotBe(second);
    }
}
