using Basirah.Application.Common.Settings;
using Basirah.Application.Interfaces.Infrastructure;
using Basirah.Application.Interfaces.Repositories;
using Basirah.Infrastructure.Identity;
using Basirah.Infrastructure.Persistence;
using Basirah.Infrastructure.Persistence.Repositories;
using Basirah.Infrastructure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Basirah.Infrastructure;

/// <summary>
/// Infrastructure layer dependency injection configuration.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Register Infrastructure layer services.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Register settings
        services.Configure<MongoDbSettings>(configuration.GetSection(MongoDbSettings.SectionName));
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        // Register MongoDB mappings
        MongoMappings.Register();

        // Register MongoDB context
        services.AddSingleton<IMongoDbContext, MongoDbContext>();

        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        // Register infrastructure services
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<IImageStorageService, LocalImageStorageService>();

        return services;
    }
}
