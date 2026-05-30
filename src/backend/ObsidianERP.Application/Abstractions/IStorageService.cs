namespace ObsidianERP.Application.Abstractions;

/// <summary>
/// Abstração de armazenamento de objetos/arquivos. A implementação atual é local,
/// mas o contrato foi pensado para uma futura troca por S3/Blob Storage sem afetar
/// a aplicação.
/// </summary>
public interface IStorageService
{
    /// <summary>Envia o conteúdo e devolve o identificador/localização do objeto.</summary>
    Task<string> UploadAsync(
        string key,
        Stream content,
        string contentType,
        CancellationToken cancellationToken = default);

    /// <summary>Recupera o conteúdo de um objeto, ou <c>null</c> se não existir.</summary>
    Task<Stream?> DownloadAsync(string key, CancellationToken cancellationToken = default);

    Task DeleteAsync(string key, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
}
