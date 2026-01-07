using Boilerplate.Application;
using Boilerplate.Infrastructure;
using Boilerplate.Presentation.Extensions;
using Boilerplate.Presentation.Middleware;

var builder = WebApplication.CreateBuilder(args);

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

// 2. Swagger (development only)
app.UseSwaggerDocumentation(app.Environment);

// 3. CORS
app.UseCorsPolicy();

// 4. Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// 5. Map controllers
app.MapControllers();

// ============================================
// Run
// ============================================
app.Run();
