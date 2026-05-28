using FluentAssertions;
using ObsidianERP.Domain.Entities;
using ObsidianERP.Domain.Exceptions;

namespace ObsidianERP.Tests.Orders;

public class OrderTests
{
    private static OrderItem Item(int quantity, decimal unitPrice, string name = "Produto") =>
        new() { ProductName = name, Quantity = quantity, UnitPrice = unitPrice };

    [Fact]
    public void Create_sem_itens_lanca_EmptyOrderException()
    {
        var act = () => Order.Create(Guid.NewGuid(), []);

        act.Should().Throw<EmptyOrderException>();
    }

    [Fact]
    public void Create_calcula_o_total_automaticamente_e_inicia_pendente()
    {
        var order = Order.Create(Guid.NewGuid(), [Item(2, 10m), Item(1, 5m)]);

        order.Total.Should().Be(25m);
        order.Status.Should().Be(OrderStatus.Pending);
        order.StatusHistory.Should().ContainSingle();
    }

    [Fact]
    public void AddItem_recalcula_o_total()
    {
        var order = Order.Create(Guid.NewGuid(), [Item(1, 10m)]);

        order.AddItem(Item(3, 5m));

        order.Total.Should().Be(25m);
        order.Items.Should().HaveCount(2);
    }

    [Fact]
    public void Cancel_define_status_cancelado_e_registra_historico()
    {
        var order = Order.Create(Guid.NewGuid(), [Item(1, 10m)]);

        order.Cancel();

        order.Status.Should().Be(OrderStatus.Cancelled);
        order.StatusHistory.Should().HaveCount(2);
    }

    [Fact]
    public void AddItem_em_pedido_cancelado_lanca_OrderCannotBeModified()
    {
        var order = Order.Create(Guid.NewGuid(), [Item(1, 10m)]);
        order.Cancel();

        order.Invoking(o => o.AddItem(Item(1, 1m)))
            .Should().Throw<OrderCannotBeModifiedException>();
    }

    [Fact]
    public void ChangeStatus_em_pedido_cancelado_lanca_OrderCannotBeModified()
    {
        var order = Order.Create(Guid.NewGuid(), [Item(1, 10m)]);
        order.Cancel();

        order.Invoking(o => o.ChangeStatus(OrderStatus.Confirmed))
            .Should().Throw<OrderCannotBeModifiedException>();
    }

    [Fact]
    public void Cancel_pedido_ja_cancelado_lanca_OrderAlreadyCancelled()
    {
        var order = Order.Create(Guid.NewGuid(), [Item(1, 10m)]);
        order.Cancel();

        order.Invoking(o => o.Cancel())
            .Should().Throw<OrderAlreadyCancelledException>();
    }

    [Fact]
    public void RemoveItem_que_esvaziaria_o_pedido_lanca_EmptyOrder()
    {
        var order = Order.Create(Guid.NewGuid(), [Item(1, 10m)]);
        var itemId = order.Items.First().Id;

        order.Invoking(o => o.RemoveItem(itemId))
            .Should().Throw<EmptyOrderException>();
    }

    [Fact]
    public void ChangeStatus_atualiza_status_e_registra_historico()
    {
        var order = Order.Create(Guid.NewGuid(), [Item(1, 10m)]);

        order.ChangeStatus(OrderStatus.Confirmed);

        order.Status.Should().Be(OrderStatus.Confirmed);
        order.StatusHistory.Should().HaveCount(2);
    }
}
