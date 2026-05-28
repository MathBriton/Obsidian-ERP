using ObsidianERP.Application.Abstractions;
using ObsidianERP.Application.Common;
using ObsidianERP.Application.DTOs;
using ObsidianERP.Application.Exceptions;
using ObsidianERP.Domain.Entities;

namespace ObsidianERP.Application.Services;

public sealed class OrderService : IOrderService
{
    private readonly IOrderRepository _orders;
    private readonly ICustomerRepository _customers;

    public OrderService(IOrderRepository orders, ICustomerRepository customers)
    {
        _orders = orders;
        _customers = customers;
    }

    public async Task<PagedResult<OrderListItemDto>> GetPagedAsync(
        OrderQuery query,
        CancellationToken cancellationToken = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize is < 1 or > 100 ? 10 : query.PageSize;

        var (items, total) = await _orders.GetPagedAsync(page, pageSize, query.Status, cancellationToken);

        var dtos = items
            .Select(o => new OrderListItemDto(o.Id, o.CustomerId, o.Customer?.Name, o.Status, o.Total, o.CreatedAt))
            .ToList();

        return new PagedResult<OrderListItemDto>(dtos, page, pageSize, total);
    }

    public async Task<OrderDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _orders.GetByIdAsync(id, cancellationToken)
            ?? throw new OrderNotFoundException(id);

        return Map(order);
    }

    public async Task<OrderDto> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        var customer = await _customers.GetByIdAsync(request.CustomerId, cancellationToken)
            ?? throw new CustomerNotFoundException(request.CustomerId);

        var items = request.Items.Select(i => new OrderItem
        {
            ProductName = i.ProductName.Trim(),
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice,
        });

        var order = Order.Create(customer.Id, items);

        await _orders.AddAsync(order, cancellationToken);
        await _orders.SaveChangesAsync(cancellationToken);

        return Map(order, customer.Name);
    }

    public async Task<OrderDto> AddItemAsync(
        Guid orderId,
        AddOrderItemRequest request,
        CancellationToken cancellationToken = default)
    {
        var order = await _orders.GetByIdAsync(orderId, cancellationToken)
            ?? throw new OrderNotFoundException(orderId);

        order.AddItem(new OrderItem
        {
            ProductName = request.ProductName.Trim(),
            Quantity = request.Quantity,
            UnitPrice = request.UnitPrice,
        });

        await _orders.SaveChangesAsync(cancellationToken);
        return Map(order);
    }

    public async Task<OrderDto> ChangeStatusAsync(
        Guid orderId,
        ChangeOrderStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        var order = await _orders.GetByIdAsync(orderId, cancellationToken)
            ?? throw new OrderNotFoundException(orderId);

        order.ChangeStatus(request.Status);

        await _orders.SaveChangesAsync(cancellationToken);
        return Map(order);
    }

    public async Task<OrderDto> CancelAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _orders.GetByIdAsync(orderId, cancellationToken)
            ?? throw new OrderNotFoundException(orderId);

        order.Cancel();

        await _orders.SaveChangesAsync(cancellationToken);
        return Map(order);
    }

    private static OrderDto Map(Order order, string? customerName = null) =>
        new(
            order.Id,
            order.CustomerId,
            customerName ?? order.Customer?.Name,
            order.Status,
            order.Total,
            order.CreatedAt,
            order.Items
                .Select(i => new OrderItemDto(i.Id, i.ProductName, i.Quantity, i.UnitPrice, i.LineTotal))
                .ToList());
}
