using System.Collections.Concurrent;
using ObsidianERP.Application.Abstractions;

namespace ObsidianERP.Infrastructure.Cloud;

/// <summary>
/// Implementação in-process de <see cref="IMessageQueue"/> baseada em filas por tipo.
/// Placeholder para uma futura fila distribuída (SQS/RabbitMQ).
/// </summary>
public sealed class InMemoryMessageQueue : IMessageQueue
{
    private readonly ConcurrentDictionary<Type, ConcurrentQueue<object>> _queues = new();

    public Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(message);
        var queue = _queues.GetOrAdd(typeof(T), _ => new ConcurrentQueue<object>());
        queue.Enqueue(message);
        return Task.CompletedTask;
    }

    public Task<T?> ConsumeAsync<T>(CancellationToken cancellationToken = default)
        where T : class
    {
        if (_queues.TryGetValue(typeof(T), out var queue) && queue.TryDequeue(out var message))
        {
            return Task.FromResult((T?)message);
        }

        return Task.FromResult<T?>(null);
    }
}
