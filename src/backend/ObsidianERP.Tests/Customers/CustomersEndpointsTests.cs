using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using ObsidianERP.Application.Common;
using ObsidianERP.Application.DTOs;
using ObsidianERP.Tests.Common;

namespace ObsidianERP.Tests.Customers;

public class CustomersEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public CustomersEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var client = _factory.CreateClient();
        var email = $"cust-{Guid.NewGuid():N}@x.com";
        var register = await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest("Tester", email, "senha123"));
        var auth = await register.Content.ReadFromJsonAsync<AuthResponse>();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", auth!.AccessToken);
        return client;
    }

    [Fact]
    public async Task Listar_sem_token_retorna_401()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/customers");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Criar_retorna_201_e_permite_obter_por_id()
    {
        var client = await CreateAuthenticatedClientAsync();

        var create = await client.PostAsJsonAsync(
            "/api/customers",
            new CreateCustomerRequest("Acme", "acme@x.com", null, null));
        create.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await create.Content.ReadFromJsonAsync<CustomerDto>();

        var get = await client.GetAsync($"/api/customers/{created!.Id}");
        get.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Listar_retorna_resultado_paginado()
    {
        var client = await CreateAuthenticatedClientAsync();
        await client.PostAsJsonAsync(
            "/api/customers",
            new CreateCustomerRequest("Paginado", null, null, null));

        var response = await client.GetAsync("/api/customers?page=1&pageSize=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var paged = await response.Content.ReadFromJsonAsync<PagedResult<CustomerDto>>();
        paged!.Page.Should().Be(1);
        paged.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Excluir_faz_soft_delete_e_some_da_busca()
    {
        var client = await CreateAuthenticatedClientAsync();
        var create = await client.PostAsJsonAsync(
            "/api/customers",
            new CreateCustomerRequest("ParaExcluir", null, null, null));
        var created = await create.Content.ReadFromJsonAsync<CustomerDto>();

        var delete = await client.DeleteAsync($"/api/customers/{created!.Id}");
        delete.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var get = await client.GetAsync($"/api/customers/{created.Id}");
        get.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Obter_inexistente_retorna_404()
    {
        var client = await CreateAuthenticatedClientAsync();

        var response = await client.GetAsync($"/api/customers/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
