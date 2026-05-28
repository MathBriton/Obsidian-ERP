using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using ObsidianERP.Application.DTOs;
using ObsidianERP.Domain.Entities;
using ObsidianERP.Tests.Common;

namespace ObsidianERP.Tests.Orders;

public class OrdersEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    private readonly CustomWebApplicationFactory _factory;

    public OrdersEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var client = _factory.CreateClient();
        var email = $"order-{Guid.NewGuid():N}@x.com";
        var register = await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest("Tester", email, "senha123"));
        var auth = await register.Content.ReadFromJsonAsync<AuthResponse>();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", auth!.AccessToken);
        return client;
    }

    private static async Task<Guid> CreateCustomerAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync(
            "/api/customers",
            new CreateCustomerRequest("Cliente", null, null, null));
        var dto = await response.Content.ReadFromJsonAsync<CustomerDto>();
        return dto!.Id;
    }

    [Fact]
    public async Task Listar_sem_token_retorna_401()
    {
        var response = await _factory.CreateClient().GetAsync("/api/orders");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Criar_pedido_retorna_201_com_total_calculado()
    {
        var client = await CreateAuthenticatedClientAsync();
        var customerId = await CreateCustomerAsync(client);

        var response = await client.PostAsJsonAsync(
            "/api/orders",
            new CreateOrderRequest(customerId, [new OrderItemRequest("Produto", 2, 10m)]),
            JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var order = await response.Content.ReadFromJsonAsync<OrderDto>(JsonOptions);
        order!.Total.Should().Be(20m);
        order.Items.Should().ContainSingle();
        order.Status.Should().Be(OrderStatus.Pending);
    }

    [Fact]
    public async Task Criar_pedido_sem_itens_retorna_400()
    {
        var client = await CreateAuthenticatedClientAsync();
        var customerId = await CreateCustomerAsync(client);

        var response = await client.PostAsJsonAsync(
            "/api/orders",
            new CreateOrderRequest(customerId, []),
            JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Cancelar_e_depois_alterar_status_retorna_409()
    {
        var client = await CreateAuthenticatedClientAsync();
        var customerId = await CreateCustomerAsync(client);
        var create = await client.PostAsJsonAsync(
            "/api/orders",
            new CreateOrderRequest(customerId, [new OrderItemRequest("P", 1, 5m)]),
            JsonOptions);
        var order = await create.Content.ReadFromJsonAsync<OrderDto>(JsonOptions);

        var cancel = await client.PostAsync($"/api/orders/{order!.Id}/cancel", null);
        cancel.StatusCode.Should().Be(HttpStatusCode.OK);

        var change = await client.PutAsJsonAsync(
            $"/api/orders/{order.Id}/status",
            new ChangeOrderStatusRequest(OrderStatus.Confirmed),
            JsonOptions);
        change.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Obter_inexistente_retorna_404()
    {
        var client = await CreateAuthenticatedClientAsync();
        var response = await client.GetAsync($"/api/orders/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Listar_retorna_resultado_paginado()
    {
        var client = await CreateAuthenticatedClientAsync();
        var customerId = await CreateCustomerAsync(client);
        await client.PostAsJsonAsync(
            "/api/orders",
            new CreateOrderRequest(customerId, [new OrderItemRequest("P", 1, 5m)]),
            JsonOptions);

        var response = await client.GetAsync("/api/orders?page=1&pageSize=5");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
