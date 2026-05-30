using System.Collections.Concurrent;
using ObsidianERP.Application.Abstractions;

namespace ObsidianERP.Infrastructure.Cloud;

/// <summary>
/// Implementação in-process de <see cref="ICacheService"/> com expiração opcional.
/// Placeholder para uma futura troca por cache distribuído (Redis).
/// </summary>
public sealed class InMemoryCacheService : ICacheService
{
    private sealed record Entry(object? Value, DateTimeOffset? ExpiresAt);

    private readonly ConcurrentDictionary<string, Entry> _entries = new();

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (_entries.TryGetValue(key, out var entry))
        {
            if (entry.ExpiresAt is { } expiresAt && expiresAt <= DateTimeOffset.UtcNow)
            {
                _entries.TryRemove(key, out _);
            }
            else if (entry.Value is T typed)
            {
                return Task.FromResult<T?>(typed);
            }
        }

        return Task.FromResult<T?>(default);
    }

    public Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? timeToLive = null,
        CancellationToken cancellationToken = default)
    {
        var expiresAt = timeToLive.HasValue ? DateTimeOffset.UtcNow.Add(timeToLive.Value) : (DateTimeOffset?)null;
        _entries[key] = new Entry(value, expiresAt);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _entries.TryRemove(key, out _);
        return Task.CompletedTask;
    }
}
