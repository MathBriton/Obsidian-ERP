using ObsidianERP.Domain.Common;

namespace ObsidianERP.Domain.Entities;

public class OrderStatusHistory : BaseEntity
{
    public Guid OrderId { get; set; }
    public OrderStatus Status { get; set; }
}
