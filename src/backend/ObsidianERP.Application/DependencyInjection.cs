using Microsoft.Extensions.DependencyInjection;

namespace ObsidianERP.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Composition root for the Application layer (services, validators).
        return services;
    }
}
