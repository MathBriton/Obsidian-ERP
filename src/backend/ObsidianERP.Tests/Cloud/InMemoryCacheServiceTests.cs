using FluentAssertions;
using ObsidianERP.Infrastructure.Cloud;

namespace ObsidianERP.Tests.Cloud;

public class InMemoryCacheServiceTests
{
    private sealed record CachedCustomer(Guid Id, string Name);

    [Fact]
    public async Task Set_seguido_de_Get_devolve_o_valor()
    {
        var sut = new InMemoryCacheService();
        var value = new CachedCustomer(Guid.NewGuid(), "Acme");

        await sut.SetAsync("cliente:1", value);

        (await sut.GetAsync<CachedCustomer>("cliente:1")).Should().Be(value);
    }

    [Fact]
    public async Task Get_de_chave_ausente_retorna_default()
    {
        var sut = new InMemoryCacheService();

        (await sut.GetAsync<CachedCustomer>("inexistente")).Should().BeNull();
    }

    [Fact]
    public async Task Remove_apaga_o_valor()
    {
        var sut = new InMemoryCacheService();
        await sut.SetAsync("k", "v");

        await sut.RemoveAsync("k");

        (await sut.GetAsync<string>("k")).Should().BeNull();
    }

    [Fact]
    public async Task Valor_com_TTL_ja_expirado_nao_e_retornado()
    {
        var sut = new InMemoryCacheService();

        await sut.SetAsync("k", "v", TimeSpan.FromMilliseconds(-1));

        (await sut.GetAsync<string>("k")).Should().BeNull();
    }
}
