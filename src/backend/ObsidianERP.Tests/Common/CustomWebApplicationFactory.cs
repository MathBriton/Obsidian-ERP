using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ObsidianERP.Infrastructure.Persistence;

namespace ObsidianERP.Tests.Common;

/// <summary>
/// Sobe a API real em ambiente de teste, trocando o PostgreSQL por um SQLite
/// in-memory (schema criado via EnsureCreated) e injetando um segredo JWT de teste.
/// </summary>
public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    static CustomWebApplicationFactory()
    {
        // Variáveis de ambiente são lidas por WebApplication.CreateBuilder já na
        // criação do builder, antes de AddJwtAuthentication ler a configuração.
        Environment.SetEnvironmentVariable("Jwt__Issuer", "obsidian-erp-tests");
        Environment.SetEnvironmentVariable("Jwt__Audience", "obsidian-erp-tests");
        Environment.SetEnvironmentVariable("Jwt__Secret", "segredo-de-teste-com-pelo-menos-32-caracteres!!");
        Environment.SetEnvironmentVariable("Jwt__AccessTokenMinutes", "15");
        Environment.SetEnvironmentVariable("Jwt__RefreshTokenDays", "7");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var toRemove = services.Where(d =>
                d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>) ||
                d.ServiceType == typeof(DbContextOptions) ||
                d.ServiceType == typeof(ApplicationDbContext) ||
                (d.ServiceType.IsGenericType &&
                 d.ServiceType.GetGenericTypeDefinition() == typeof(IDbContextOptionsConfiguration<>)))
                .ToList();
            foreach (var descriptor in toRemove)
            {
                services.Remove(descriptor);
            }

            _connection.Open();
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(_connection));

            using var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.EnsureCreated();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection.Dispose();
        }
    }
}
