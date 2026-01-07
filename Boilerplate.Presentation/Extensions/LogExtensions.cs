using Serilog;
using Serilog.Events;

namespace Boilerplate.Presentation.Extensions;

/// <summary>
/// Serilog logging configuration.
/// Clean, robust logging with proper level filtering.
/// </summary>
public static class LogExtensions
{
    private const string OutputTemplate = 
        "[{Timestamp:yyyy-MM-dd HH:mm:ss zzz} {Level:u4}] {Message:lj} ({SourceContext}){NewLine}{Exception}";

    /// <summary>
    /// Configure Serilog with sensible defaults.
    /// - Suppresses noisy Microsoft.* logs
    /// - Keeps important lifecycle and hosting logs
    /// - Clean console output format
    /// </summary>
    public static WebApplicationBuilder AddSerilogLogging(this WebApplicationBuilder builder)
    {
        // Read log level from configuration (default: Information)
        var logLevelStr = builder.Configuration["Logging:LogLevel:Default"] ?? "Information";
        var logLevel = Enum.TryParse<LogEventLevel>(logLevelStr, out var level) 
            ? level 
            : LogEventLevel.Information;

        Log.Logger = new LoggerConfiguration()
            // Base minimum level
            .MinimumLevel.Is(logLevel)
            
            // Suppress noisy Microsoft logs (but keep important ones)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Cors", LogEventLevel.Warning)
            
            // Suppress EF/MongoDB driver noise (if you add EF later)
            .MinimumLevel.Override("MongoDB", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            
            // System noise
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .MinimumLevel.Override("System.Net.Http", LogEventLevel.Warning)
            
            // Console output with clean format
            .WriteTo.Console(outputTemplate: OutputTemplate)
            
            // Enrich with context
            .Enrich.FromLogContext()
            
            .CreateLogger();

        // Clear default providers and use Serilog
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(Log.Logger);

        // Log startup
        Log.Information("Serilog configured. Minimum level: {LogLevel}", logLevel);

        return builder;
    }

    /// <summary>
    /// Ensure proper cleanup on application shutdown.
    /// </summary>
    public static void UseSerilogCleanup(this WebApplication app)
    {
        app.Lifetime.ApplicationStopping.Register(() =>
        {
            Log.Information("Application shutting down...");
            Log.CloseAndFlush();
        });
    }
}
