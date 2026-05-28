using Microsoft.EntityFrameworkCore;
using ObsidianERP.Application.Abstractions;
using ObsidianERP.Domain.Entities;

namespace ObsidianERP.Infrastructure.Persistence.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _db;

    public OrderRepository(ApplicationDbContext db) => _db = db;

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.Orders
            .IgnoreQueryFilters()
            .Include(o => o.Items)
            .Include(o => o.StatusHistory)
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<Order> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        OrderStatus? status,
        CancellationToken cancellationToken = default)
    {
        var queryable = _db.Orders.IgnoreQueryFilters().Include(o => o.Customer).AsNoTracking();

        if (status.HasValue)
        {
            queryable = queryable.Where(o => o.Status == status.Value);
        }

        var total = await queryable.CountAsync(cancellationToken);
        var items = await queryable
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken = default) =>
        await _db.Orders.AddAsync(order, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _db.SaveChangesAsync(cancellationToken);
}
