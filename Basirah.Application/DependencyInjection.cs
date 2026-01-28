using Basirah.Application.Interfaces.Services;
using Basirah.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Basirah.Application;

/// <summary>
/// Application layer dependency injection configuration.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Register Application layer services.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register application services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();

        return services;
    }
}
