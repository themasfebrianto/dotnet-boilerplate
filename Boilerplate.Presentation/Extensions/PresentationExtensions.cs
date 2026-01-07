using Boilerplate.Application.Common.Abstractions;
using Boilerplate.Presentation.Services;

namespace Boilerplate.Presentation.Extensions;

/// <summary>
/// Presentation layer service registration extensions.
/// </summary>
public static class PresentationExtensions
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        // Add controllers with custom model state handling
        services.AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                // Let model validation errors pass through to our exception middleware
                options.SuppressModelStateInvalidFilter = true;
            });

        // Add HttpContextAccessor for CurrentUserService
        services.AddHttpContextAccessor();

        // Register CurrentUserService
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // Add CORS
        services.AddCorsPolicy(configuration);

        // Add JWT Authentication
        services.AddJwtAuthentication(configuration);

        // Add Authorization
        services.AddAuthorization();

        // Add Swagger
        services.AddSwaggerDocumentation();

        // Add OpenAPI/Endpoints
        services.AddEndpointsApiExplorer();

        return services;
    }
}
