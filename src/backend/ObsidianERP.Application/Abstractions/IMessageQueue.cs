namespace ObsidianERP.Application.Abstractions;

/// <summary>
/// Abstração de fila de mensagens para processamento assíncrono/desacoplado.
/// A implementação atual é em memória (in-process); o contrato permite trocar
/// futuramente por SQS/RabbitMQ sem mudar quem publica ou consome.
/// </summary>
public interface IMessageQueue
{
    /// <summary>Publica uma mensagem na fila do tipo <typeparamref name="T"/>.</summary>
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
        where T : class;

    /// <summary>Consome a próxima mensagem do tipo <typeparamref name="T"/>, ou <c>null</c> se a fila estiver vazia.</summary>
    Task<T?> ConsumeAsync<T>(CancellationToken cancellationToken = default)
        where T : class;
}
