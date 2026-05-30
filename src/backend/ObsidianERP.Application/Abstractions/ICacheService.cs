namespace ObsidianERP.Application.Abstractions;

/// <summary>
/// Abstração de cache chave-valor. A implementação atual é em memória; o contrato
/// foi desenhado para uma futura troca por um cache distribuído (Redis) sem afetar
/// os consumidores.
/// </summary>
public interface ICacheService
{
    /// <summary>Obtém o valor associado à chave, ou <c>default</c> se ausente/expirado.</summary>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>Grava um valor, opcionalmente com tempo de expiração (TTL).</summary>
    Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? timeToLive = null,
        CancellationToken cancellationToken = default);

    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}
