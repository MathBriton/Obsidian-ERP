using FluentAssertions;
using ObsidianERP.Infrastructure.Security;

namespace ObsidianERP.Tests.Auth;

public class BcryptPasswordHasherTests
{
    private readonly BcryptPasswordHasher _sut = new();

    [Fact]
    public void Hash_nao_retorna_a_senha_em_texto_plano()
    {
        var hash = _sut.Hash("senha123");

        hash.Should().NotBe("senha123");
        hash.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Verify_retorna_true_para_a_senha_correta()
    {
        var hash = _sut.Hash("senha123");

        _sut.Verify("senha123", hash).Should().BeTrue();
    }

    [Fact]
    public void Verify_retorna_false_para_a_senha_incorreta()
    {
        var hash = _sut.Hash("senha123");

        _sut.Verify("outra-senha", hash).Should().BeFalse();
    }
}
