namespace Boilerplate.Presentation.Extensions;

/// <summary>
/// CORS configuration extensions.
/// </summary>
public static class CorsExtensions
{
    public const string PolicyName = "DefaultCorsPolicy";

    public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? ["*"];

        services.AddCors(options =>
        {
            options.AddPolicy(PolicyName, policy =>
            {
                if (allowedOrigins.Contains("*"))
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                }
                else
                {
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                }
            });
        });

        return services;
    }

    public static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app)
        => app.UseCors(PolicyName);
}
