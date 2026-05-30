using System.Text;
using FluentAssertions;
using ObsidianERP.Infrastructure.Cloud;

namespace ObsidianERP.Tests.Cloud;

public class LocalFileStorageServiceTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), $"obsidian-storage-{Guid.NewGuid():N}");
    private readonly LocalFileStorageService _sut;

    public LocalFileStorageServiceTests()
    {
        _sut = new LocalFileStorageService(_root);
    }

    private static Stream StreamOf(string content) => new MemoryStream(Encoding.UTF8.GetBytes(content));

    [Fact]
    public async Task Upload_grava_e_Download_recupera_o_mesmo_conteudo()
    {
        await _sut.UploadAsync("docs/nota.txt", StreamOf("olá mundo"), "text/plain");

        var stream = await _sut.DownloadAsync("docs/nota.txt");
        stream.Should().NotBeNull();
        using var reader = new StreamReader(stream!);
        (await reader.ReadToEndAsync()).Should().Be("olá mundo");
    }

    [Fact]
    public async Task Exists_reflete_a_presenca_do_objeto()
    {
        (await _sut.ExistsAsync("ausente.txt")).Should().BeFalse();

        await _sut.UploadAsync("presente.txt", StreamOf("x"), "text/plain");

        (await _sut.ExistsAsync("presente.txt")).Should().BeTrue();
    }

    [Fact]
    public async Task Download_de_chave_inexistente_retorna_null()
    {
        (await _sut.DownloadAsync("nao-existe.bin")).Should().BeNull();
    }

    [Fact]
    public async Task Delete_remove_o_objeto()
    {
        await _sut.UploadAsync("temp.txt", StreamOf("x"), "text/plain");

        await _sut.DeleteAsync("temp.txt");

        (await _sut.ExistsAsync("temp.txt")).Should().BeFalse();
    }

    [Fact]
    public async Task Upload_devolve_um_identificador_nao_vazio()
    {
        var key = await _sut.UploadAsync("a/b/c.txt", StreamOf("x"), "text/plain");

        key.Should().NotBeNullOrWhiteSpace();
    }

    public void Dispose()
    {
        if (Directory.Exists(_root))
        {
            Directory.Delete(_root, recursive: true);
        }
    }
}
