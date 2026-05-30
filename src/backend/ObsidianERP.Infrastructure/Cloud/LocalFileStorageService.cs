using ObsidianERP.Application.Abstractions;

namespace ObsidianERP.Infrastructure.Cloud;

/// <summary>
/// Implementação local de <see cref="IStorageService"/> que grava os objetos no
/// sistema de arquivos. Placeholder para uma futura implementação em S3/Blob Storage.
/// </summary>
public sealed class LocalFileStorageService : IStorageService
{
    private readonly string _root;

    public LocalFileStorageService(string root)
    {
        _root = Path.GetFullPath(root);
        Directory.CreateDirectory(_root);
    }

    public async Task<string> UploadAsync(
        string key,
        Stream content,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var path = ResolvePath(key);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        await using var file = File.Create(path);
        await content.CopyToAsync(file, cancellationToken);

        return key;
    }

    public Task<Stream?> DownloadAsync(string key, CancellationToken cancellationToken = default)
    {
        var path = ResolvePath(key);
        if (!File.Exists(path))
        {
            return Task.FromResult<Stream?>(null);
        }

        Stream stream = File.OpenRead(path);
        return Task.FromResult<Stream?>(stream);
    }

    public Task DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        var path = ResolvePath(key);
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        => Task.FromResult(File.Exists(ResolvePath(key)));

    private string ResolvePath(string key)
    {
        // Normaliza a chave (ex.: "docs/nota.txt") em um caminho seguro sob a raiz,
        // impedindo escape do diretório (path traversal).
        var relative = key.Replace('\\', '/').TrimStart('/');
        var combined = Path.GetFullPath(Path.Combine(_root, relative));

        // Compara com a raiz acrescida do separador para impedir bypass do tipo
        // "/dados/storage" vs "/dados/storage-evil".
        var prefix = _root.EndsWith(Path.DirectorySeparatorChar)
            ? _root
            : _root + Path.DirectorySeparatorChar;
        if (!combined.StartsWith(prefix, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Chave inválida: '{key}'.");
        }

        return combined;
    }
}
