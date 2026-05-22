using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using ObsidianERP.Tests.Common;

namespace ObsidianERP.Tests.Api;

public class SwaggerEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public SwaggerEndpointTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Swagger_document_is_available_in_development()
    {
        var client = _factory
            .WithWebHostBuilder(builder => builder.UseEnvironment("Development"))
            .CreateClient();

        var response = await client.GetAsync("/swagger/v1/swagger.json");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("Obsidian ERP API");
    }
}
