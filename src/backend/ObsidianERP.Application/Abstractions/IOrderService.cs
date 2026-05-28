using ObsidianERP.Application.Common;
using ObsidianERP.Application.DTOs;

namespace ObsidianERP.Application.Abstractions;

public interface IOrderService
{
    Task<PagedResult<OrderListItemDto>> GetPagedAsync(OrderQuery query, CancellationToken cancellationToken = default);
    Task<OrderDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<OrderDto> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);
    Task<OrderDto> AddItemAsync(Guid orderId, AddOrderItemRequest request, CancellationToken cancellationToken = default);
    Task<OrderDto> ChangeStatusAsync(Guid orderId, ChangeOrderStatusRequest request, CancellationToken cancellationToken = default);
    Task<OrderDto> CancelAsync(Guid orderId, CancellationToken cancellationToken = default);
}
