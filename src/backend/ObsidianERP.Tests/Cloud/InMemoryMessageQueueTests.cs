using FluentAssertions;
using ObsidianERP.Infrastructure.Cloud;

namespace ObsidianERP.Tests.Cloud;

public class InMemoryMessageQueueTests
{
    private sealed record OrderCreated(Guid OrderId);
    private sealed record EmailRequested(string To);

    [Fact]
    public async Task Publish_seguido_de_Consume_devolve_a_mensagem()
    {
        var sut = new InMemoryMessageQueue();
        var message = new OrderCreated(Guid.NewGuid());

        await sut.PublishAsync(message);

        (await sut.ConsumeAsync<OrderCreated>()).Should().Be(message);
    }

    [Fact]
    public async Task Consume_de_fila_vazia_retorna_null()
    {
        var sut = new InMemoryMessageQueue();

        (await sut.ConsumeAsync<OrderCreated>()).Should().BeNull();
    }

    [Fact]
    public async Task Mensagens_sao_consumidas_em_ordem_FIFO()
    {
        var sut = new InMemoryMessageQueue();
        var first = new OrderCreated(Guid.NewGuid());
        var second = new OrderCreated(Guid.NewGuid());

        await sut.PublishAsync(first);
        await sut.PublishAsync(second);

        (await sut.ConsumeAsync<OrderCreated>()).Should().Be(first);
        (await sut.ConsumeAsync<OrderCreated>()).Should().Be(second);
    }

    [Fact]
    public async Task Filas_sao_isoladas_por_tipo()
    {
        var sut = new InMemoryMessageQueue();
        await sut.PublishAsync(new OrderCreated(Guid.NewGuid()));

        (await sut.ConsumeAsync<EmailRequested>()).Should().BeNull();
    }
}
