using FluentAssertions;
using NSubstitute;
using ObsidianERP.Application.Abstractions;
using ObsidianERP.Application.DTOs;
using ObsidianERP.Application.Exceptions;
using ObsidianERP.Application.Services;
using ObsidianERP.Domain.Entities;

namespace ObsidianERP.Tests.Orders;

public class OrderServiceTests
{
    private readonly IOrderRepository _orders = Substitute.For<IOrderRepository>();
    private readonly ICustomerRepository _customers = Substitute.For<ICustomerRepository>();

    private OrderService CreateSut() => new(_orders, _customers);

    [Fact]
    public async Task CreateAsync_com_cliente_valido_cria_pedido_com_total()
    {
        var customer = new Customer { Name = "Acme" };
        _customers.GetByIdAsync(customer.Id, Arg.Any<CancellationToken>()).Returns(customer);

        var request = new CreateOrderRequest(customer.Id, [new OrderItemRequest("Produto", 2, 10m)]);
        var dto = await CreateSut().CreateAsync(request);

        dto.Total.Should().Be(20m);
        dto.Status.Should().Be(OrderStatus.Pending);
        dto.CustomerName.Should().Be("Acme");
        await _orders.Received(1).AddAsync(Arg.Any<Order>(), Arg.Any<CancellationToken>());
        await _orders.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_com_cliente_inexistente_lanca_CustomerNotFound()
    {
        _customers.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Customer?)null);

        var request = new CreateOrderRequest(Guid.NewGuid(), [new OrderItemRequest("P", 1, 1m)]);

        await CreateSut()
            .Invoking(s => s.CreateAsync(request))
            .Should().ThrowAsync<CustomerNotFoundException>();
    }

    [Fact]
    public async Task GetByIdAsync_inexistente_lanca_OrderNotFound()
    {
        _orders.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Order?)null);

        await CreateSut()
            .Invoking(s => s.GetByIdAsync(Guid.NewGuid()))
            .Should().ThrowAsync<OrderNotFoundException>();
    }

    [Fact]
    public async Task CancelAsync_cancela_e_persiste()
    {
        var order = Order.Create(
            Guid.NewGuid(),
            [new OrderItem { ProductName = "P", Quantity = 1, UnitPrice = 5m }]);
        _orders.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);

        var dto = await CreateSut().CancelAsync(order.Id);

        dto.Status.Should().Be(OrderStatus.Cancelled);
        await _orders.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
