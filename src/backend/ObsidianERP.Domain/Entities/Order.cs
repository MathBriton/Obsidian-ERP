using ObsidianERP.Domain.Common;
using ObsidianERP.Domain.Exceptions;

namespace ObsidianERP.Domain.Entities;

public class Order : BaseEntity
{
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public decimal Total { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public Customer? Customer { get; private set; }
    public List<OrderItem> Items { get; private set; } = [];
    public List<OrderStatusHistory> StatusHistory { get; private set; } = [];

    // Construtor sem parâmetros para materialização do EF Core.
    private Order()
    {
    }

    public static Order Create(Guid customerId, IEnumerable<OrderItem> items)
    {
        var list = items?.ToList() ?? [];
        if (list.Count == 0)
        {
            throw new EmptyOrderException();
        }

        var order = new Order
        {
            CustomerId = customerId,
            Status = OrderStatus.Pending,
        };
        order.Items.AddRange(list);
        order.RecalculateTotal();
        order.RecordStatus(OrderStatus.Pending);
        return order;
    }

    public void AddItem(OrderItem item)
    {
        EnsureModifiable();
        Items.Add(item);
        RecalculateTotal();
        Touch();
    }

    public void RemoveItem(Guid itemId)
    {
        EnsureModifiable();

        var item = Items.FirstOrDefault(i => i.Id == itemId);
        if (item is null)
        {
            return;
        }

        if (Items.Count == 1)
        {
            throw new EmptyOrderException();
        }

        Items.Remove(item);
        RecalculateTotal();
        Touch();
    }

    public void ChangeStatus(OrderStatus status)
    {
        EnsureModifiable();
        if (status == Status)
        {
            return;
        }

        Status = status;
        RecordStatus(status);
        Touch();
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Cancelled)
        {
            throw new OrderAlreadyCancelledException();
        }

        Status = OrderStatus.Cancelled;
        RecordStatus(OrderStatus.Cancelled);
        Touch();
    }

    private void EnsureModifiable()
    {
        if (Status == OrderStatus.Cancelled)
        {
            throw new OrderCannotBeModifiedException();
        }
    }

    private void RecalculateTotal() => Total = Items.Sum(i => i.LineTotal);

    private void RecordStatus(OrderStatus status) =>
        StatusHistory.Add(new OrderStatusHistory { Status = status });

    private void Touch() => UpdatedAt = DateTime.UtcNow;
}
