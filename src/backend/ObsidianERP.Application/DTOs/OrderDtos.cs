using ObsidianERP.Domain.Entities;

namespace ObsidianERP.Application.DTOs;

public sealed record OrderItemRequest(string ProductName, int Quantity, decimal UnitPrice);

public sealed record CreateOrderRequest(Guid CustomerId, List<OrderItemRequest> Items);

public sealed record AddOrderItemRequest(string ProductName, int Quantity, decimal UnitPrice);

public sealed record ChangeOrderStatusRequest(OrderStatus Status);

public sealed record OrderItemDto(
    Guid Id,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal);

public sealed record OrderDto(
    Guid Id,
    Guid CustomerId,
    string? CustomerName,
    OrderStatus Status,
    decimal Total,
    DateTime CreatedAt,
    IReadOnlyList<OrderItemDto> Items);

public sealed record OrderListItemDto(
    Guid Id,
    Guid CustomerId,
    string? CustomerName,
    OrderStatus Status,
    decimal Total,
    DateTime CreatedAt);

public sealed record OrderQuery(int Page = 1, int PageSize = 10, OrderStatus? Status = null);
