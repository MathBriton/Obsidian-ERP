using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using ObsidianERP.Application.DTOs;
using ObsidianERP.Tests.Common;

namespace ObsidianERP.Tests.Quality;

/// <summary>
/// Garante que os erros seguem o contrato RFC 7807 (ProblemDetails / application/problem+json),
/// produzido de forma centralizada pelo GlobalExceptionHandler e pelo filtro de validação.
/// </summary>
public class ErrorContractTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public ErrorContractTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var client = _factory.CreateClient();
        var register = await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest("Tester", $"err-{Guid.NewGuid():N}@x.com", "senha123"));
        var auth = await register.Content.ReadFromJsonAsync<AuthResponse>();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth!.AccessToken);
        return client;
    }

    [Fact]
    public async Task Nao_encontrado_retorna_problem_details_404()
    {
        var client = await CreateAuthenticatedClientAsync();

        var response = await client.GetAsync($"/api/customers/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");
        var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        doc.RootElement.GetProperty("status").GetInt32().Should().Be(404);
        doc.RootElement.GetProperty("title").GetString().Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Conflito_retorna_problem_details_409()
    {
        var client = _factory.CreateClient();
        var request = new RegisterRequest("Ana", $"conflict-{Guid.NewGuid():N}@x.com", "senha123");
        await client.PostAsJsonAsync("/api/auth/register", request);

        var second = await client.PostAsJsonAsync("/api/auth/register", request);

        second.StatusCode.Should().Be(HttpStatusCode.Conflict);
        second.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");
        var doc = await JsonDocument.ParseAsync(await second.Content.ReadAsStreamAsync());
        doc.RootElement.GetProperty("status").GetInt32().Should().Be(409);
    }

    [Fact]
    public async Task Nao_autorizado_retorna_problem_details_401()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest("ninguem@x.com", "senha123"));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");
    }

    [Fact]
    public async Task Validacao_retorna_problem_details_400_com_erros()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest("", "nao-e-email", "123"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");
        var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        doc.RootElement.TryGetProperty("errors", out var errors).Should().BeTrue();
        errors.EnumerateObject().Should().NotBeEmpty();
    }
}
