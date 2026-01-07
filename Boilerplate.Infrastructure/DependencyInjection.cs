using Boilerplate.Application.Common.Settings;
using Boilerplate.Application.Interfaces.Infrastructure;
using Boilerplate.Application.Interfaces.Repositories;
using Boilerplate.Infrastructure.Identity;
using Boilerplate.Infrastructure.Persistence;
using Boilerplate.Infrastructure.Persistence.Repositories;
using Boilerplate.Infrastructure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Boilerplate.Infrastructure;

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
