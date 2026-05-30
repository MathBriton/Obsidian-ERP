using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ObsidianERP.Application.Abstractions;
using ObsidianERP.Infrastructure.Cloud;
using ObsidianERP.Infrastructure.Persistence;
using ObsidianERP.Infrastructure.Persistence.Repositories;
using ObsidianERP.Infrastructure.Security;

namespace ObsidianERP.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        // Abstrações cloud-prep (Stage 7) — implementações locais, prontas para troca
        // futura por S3/SQS/Redis sem afetar a aplicação.
        var storageRoot = configuration["Storage:LocalRootPath"];
        if (string.IsNullOrWhiteSpace(storageRoot))
        {
            storageRoot = Path.Combine(AppContext.BaseDirectory, "storage");
        }
        services.AddSingleton<IStorageService>(_ => new LocalFileStorageService(storageRoot));
        services.AddSingleton<IMessageQueue, InMemoryMessageQueue>();
        services.AddSingleton<ICacheService, InMemoryCacheService>();

        return services;
    }
}
