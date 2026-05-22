using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ObsidianERP.Application.Abstractions;
using ObsidianERP.Application.Services;

namespace ObsidianERP.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        return services;
    }
}
