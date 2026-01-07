using Boilerplate.Application;
using Boilerplate.Infrastructure;
using Boilerplate.Presentation.Extensions;
using Boilerplate.Presentation.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// Logging (must be first for early errors)
// ============================================
builder.AddSerilogLogging();

// ============================================
// Service Registration (each layer owns its DI)
// ============================================
builder.Services
    .AddPresentation(builder.Configuration)  // Presentation layer (controllers, auth, swagger)
    .AddApplication()                         // Application layer (services)
    .AddInfrastructure(builder.Configuration); // Infrastructure layer (repositories, providers)

var app = builder.Build();

// ============================================
// Middleware Pipeline (order matters!)
// ============================================

// 1. Exception handling (must be first to catch all exceptions)
app.UseExceptionMiddleware();

// 2. Serilog request logging (after exception middleware)
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
});

// 3. Swagger (development only)
app.UseSwaggerDocumentation(app.Environment);

// 4. CORS
app.UseCorsPolicy();

// 5. Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// 6. Map controllers
app.MapControllers();

// 7. Serilog cleanup on shutdown
app.UseSerilogCleanup();

// ============================================
// Run
// ============================================
Log.Information("Starting Boilerplate API...");

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
