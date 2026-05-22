using Microsoft.EntityFrameworkCore;
using ObsidianERP.Application.Abstractions;
using ObsidianERP.Domain.Entities;

namespace ObsidianERP.Infrastructure.Persistence.Repositories;

public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _db;

    public RefreshTokenRepository(ApplicationDbContext db) => _db = db;

    public Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default) =>
        _db.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == token, cancellationToken);

    public async Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default) =>
        await _db.RefreshTokens.AddAsync(token, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _db.SaveChangesAsync(cancellationToken);
}
