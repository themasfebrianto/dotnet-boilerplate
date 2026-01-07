# .NET Clean Architecture Boilerplate

## Philosophy: Pragmatic Exception-Driven Flow

**One rule. No exceptions (pun intended).**

| Layer | Responsibility |
|-------|---------------|
| **Application** | Throw intentful exceptions |
| **Controller** | Call service → return success (3-5 lines, always) |
| **Middleware** | Catch exception → map to HTTP |
| **Response** | Wrapped centrally (never in controllers) |

**What we DON'T have:**
- ❌ Result<T> pattern
- ❌ Controller branching on success/failure
- ❌ Dual error systems
- ❌ Manual response wrapping in controllers
- ❌ MediatR/CQRS ceremony

**What we DO have:**
- ✅ Typed exceptions with semantic meaning
- ✅ One global middleware that handles ALL HTTP mapping
- ✅ Controllers that assume success (if it reached here, it's valid)
- ✅ Automatic response wrapping
- ✅ 3-5 line controller methods

---

## Directory Structure

```
dotnet-boilerplate/
├── Boilerplate.Application/
│   ├── Common/
│   │   ├── Abstractions/
│   │   │   └── ICurrentUserService.cs
│   │   ├── Exceptions/
│   │   │   ├── AppException.cs              # Base class with ErrorCode
│   │   │   ├── NotFoundException.cs         # → 404
│   │   │   ├── ValidationException.cs       # → 400
│   │   │   ├── ConflictException.cs         # → 409
│   │   │   ├── UnauthorizedException.cs     # → 401
│   │   │   └── ForbiddenException.cs        # → 403
│   │   └── Settings/
│   │       ├── JwtSettings.cs
│   │       └── MongoDbSettings.cs
│   ├── DTOs/
│   │   ├── Auth/
│   │   │   ├── LoginRequestDto.cs
│   │   │   └── LoginResponseDto.cs
│   │   ├── Role/
│   │   │   └── RoleDto.cs
│   │   └── User/
│   │       ├── UserRequestDto.cs
│   │       └── UserResponseDto.cs
│   ├── Interfaces/
│   │   ├── Services/
│   │   │   ├── IAuthService.cs
│   │   │   ├── IRoleService.cs
│   │   │   └── IUserService.cs
│   │   ├── Repositories/
│   │   │   ├── IRefreshTokenRepository.cs
│   │   │   ├── IRoleRepository.cs
│   │   │   └── IUserRepository.cs
│   │   └── Infrastructure/
│   │       ├── IJwtProvider.cs
│   │       └── IImageStorageService.cs
│   ├── Mappings/
│   │   ├── UserMappingExtensions.cs
│   │   └── RoleMappingExtensions.cs
│   ├── Services/
│   │   ├── AuthService.cs
│   │   ├── RoleService.cs
│   │   └── UserService.cs
│   ├── DependencyInjection.cs
│   └── Boilerplate.Application.csproj
│
├── Boilerplate.Domain/
│   ├── Common/
│   │   ├── BaseEntity.cs
│   │   └── Interfaces/
│   │       ├── ISoftDeletable.cs         # Composable soft-delete
│   │       └── IAuditable.cs             # Composable audit tracking
│   ├── Entities/
│   │   ├── User.cs        : BaseEntity, ISoftDeletable, IAuditable
│   │   ├── Role.cs        : BaseEntity, ISoftDeletable
│   │   └── RefreshToken.cs : BaseEntity  # No extras needed
│   └── Boilerplate.Domain.csproj
│
├── Boilerplate.Infrastructure/
│   ├── Identity/
│   │   └── JwtProvider.cs
│   ├── Persistence/
│   │   ├── Common/
│   │   │   ├── FilterDefinitionBuilder.cs
│   │   │   └── UpdateDefinitionBuilder.cs
│   │   ├── MongoDbContext.cs
│   │   ├── MongoMappings.cs
│   │   └── Repositories/
│   │       ├── RefreshTokenRepository.cs
│   │       ├── RoleRepository.cs
│   │       └── UserRepository.cs
│   ├── Storage/
│   │   └── ImageStorageService.cs
│   ├── DependencyInjection.cs
│   └── Boilerplate.Infrastructure.csproj
│
├── Boilerplate.Presentation/
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── RoleController.cs
│   │   └── UserController.cs
│   ├── Extensions/
│   │   ├── CorsExtensions.cs
│   │   ├── JwtExtensions.cs
│   │   ├── SettingsExtensions.cs
│   │   └── SwaggerExtensions.cs
│   ├── Middleware/
│   │   └── ExceptionMiddleware.cs
│   ├── Services/
│   │   └── CurrentUserService.cs
│   ├── Program.cs
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   └── Boilerplate.Presentation.csproj
│
├── Boilerplate.Tests/
│   └── Boilerplate.Tests.csproj
│
├── Boilerplate.sln
├── .gitignore
├── .env.example
└── README.md
```

---

## Core Contracts

### 1. AppException (Base Class)

```csharp
// Boilerplate.Application/Common/Exceptions/AppException.cs
namespace Boilerplate.Application.Common.Exceptions;

public abstract class AppException : Exception
{
    public abstract int StatusCode { get; }
    public virtual string ErrorCode => GetType().Name.Replace("Exception", "");
    
    protected AppException(string message) : base(message) { }
    protected AppException(string message, Exception inner) : base(message, inner) { }
}
```

### 2. Typed Exceptions

```csharp
// NotFoundException.cs → 404
public class NotFoundException : AppException
{
    public override int StatusCode => 404;
    public NotFoundException(string entity, object id) 
        : base($"{entity} with ID '{id}' was not found.") { }
}

// ValidationException.cs → 400
public class ValidationException : AppException
{
    public override int StatusCode => 400;
    public ValidationException(string message) : base(message) { }
}

// ConflictException.cs → 409
public class ConflictException : AppException
{
    public override int StatusCode => 409;
    public ConflictException(string message) : base(message) { }
}

// UnauthorizedException.cs → 401
public class UnauthorizedException : AppException
{
    public override int StatusCode => 401;
    public UnauthorizedException(string message = "Invalid credentials.") : base(message) { }
}

// ForbiddenException.cs → 403
public class ForbiddenException : AppException
{
    public override int StatusCode => 403;
    public ForbiddenException(string message = "Access denied.") : base(message) { }
}
```

### 3. ExceptionMiddleware (The Heart)

```csharp
// Boilerplate.Presentation/Middleware/ExceptionMiddleware.cs
namespace Boilerplate.Presentation.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (AppException ex)
        {
            // Expected business failure - just log as warning
            logger.LogWarning("Business exception: {Message}", ex.Message);
            await WriteResponse(context, ex.StatusCode, ex.Message, ex.ErrorCode);
        }
        catch (Exception ex)
        {
            // Unexpected failure - log as error with trace
            var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
            logger.LogError(ex, "Unhandled exception. TraceId: {TraceId}", traceId);
            await WriteResponse(context, 500, "An unexpected error occurred.", "ServerError", traceId);
        }
    }

    private static async Task WriteResponse(
        HttpContext context, 
        int statusCode, 
        string message, 
        string errorCode,
        string? traceId = null)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        
        await context.Response.WriteAsJsonAsync(new
        {
            success = false,
            message,
            errorCode,
            traceId
        });
    }
}

// Extension method
public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder app)
        => app.UseMiddleware<ExceptionMiddleware>();
}
```

### 4. Response Wrapping Middleware

```csharp
// Boilerplate.Presentation/Middleware/ResponseWrapperMiddleware.cs
namespace Boilerplate.Presentation.Middleware;

public class ResponseWrapperMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var originalBody = context.Response.Body;
        using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        await next(context);

        memoryStream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();
        memoryStream.Seek(0, SeekOrigin.Begin);

        context.Response.Body = originalBody;

        // Only wrap successful JSON responses
        if (context.Response.StatusCode >= 200 && 
            context.Response.StatusCode < 300 &&
            context.Response.ContentType?.Contains("application/json") == true)
        {
            var wrapped = new
            {
                success = true,
                data = JsonSerializer.Deserialize<object>(responseBody)
            };
            await context.Response.WriteAsJsonAsync(wrapped);
        }
        else
        {
            await memoryStream.CopyToAsync(originalBody);
        }
    }
}
```

### 5. Service Pattern (Throw, Never Return Null)

```csharp
// Boilerplate.Application/Services/AuthService.cs
namespace Boilerplate.Application.Services;

public class AuthService(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IJwtProvider jwtProvider,
    IOptions<JwtSettings> jwtSettings) : IAuthService
{
    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await userRepository.GetByEmailAsync(request.Email)
            ?? throw new UnauthorizedException();

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException();

        var accessToken = jwtProvider.GenerateToken(user);
        var refreshToken = jwtProvider.GenerateRefreshToken();
        var refreshExpiry = DateTime.UtcNow.Add(jwtSettings.Value.RefreshTokenExpiration);

        await refreshTokenRepository.UpdateRefreshTokenAsync(user.Id!.Value, refreshToken, refreshExpiry);

        return new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpires = DateTime.UtcNow.Add(jwtSettings.Value.Expiration),
            RefreshTokenExpires = refreshExpiry
        };
    }

    public async Task<LoginResponseDto> RegisterAsync(UserRequestDto request)
    {
        var existing = await userRepository.GetByEmailAsync(request.Email);
        if (existing is not null)
            throw new ConflictException("Email already registered.");

        // ... create user, generate tokens
        // If anything fails, THROW. Never return null.
    }
}
```

### 6. Controller Pattern (3-5 Lines, Always)

```csharp
// Boilerplate.Presentation/Controllers/AuthController.cs
namespace Boilerplate.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<LoginResponseDto> Login([FromBody] LoginRequestDto request)
        => await authService.LoginAsync(request);

    [HttpPost("register")]
    public async Task<LoginResponseDto> Register([FromBody] UserRequestDto request)
        => await authService.RegisterAsync(request);

    [HttpPost("refresh")]
    public async Task<LoginResponseDto> Refresh([FromBody] RefreshTokenRequestDto request)
        => await authService.RefreshTokenAsync(request.RefreshToken);
}
```

**Notice:**
- No `IActionResult`
- No `try-catch`
- No `if (result == null)`
- No response wrapping
- Just call → return

### 7. ICurrentUserService

```csharp
// Boilerplate.Application/Common/Abstractions/ICurrentUserService.cs
namespace Boilerplate.Application.Common.Abstractions;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Email { get; }
    IEnumerable<string> Roles { get; }
    bool IsAuthenticated { get; }
}

// Boilerplate.Presentation/Services/CurrentUserService.cs
namespace Boilerplate.Presentation.Services;

public class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUserService
{
    private ClaimsPrincipal? User => accessor.HttpContext?.User;
    
    public Guid? UserId => Guid.TryParse(
        User?.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
    
    public string? Email => User?.FindFirstValue(ClaimTypes.Email);
    
    public IEnumerable<string> Roles => User?
        .FindAll(ClaimTypes.Role)
        .Select(c => c.Value) ?? [];
    
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
}
```

### 8. Program.cs (Clean Pipeline)

```csharp
// Boilerplate.Presentation/Program.cs
using Boilerplate.Application;
using Boilerplate.Infrastructure;
using Boilerplate.Presentation.Middleware;
using Boilerplate.Presentation.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Layer registrations (each layer owns its DI)
builder.Services
    .AddPresentation(builder.Configuration)
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Middleware pipeline (order matters)
app.UseExceptionMiddleware();      // 1. Catch all exceptions
app.UseSwaggerApp();               // 2. Swagger (dev only)
app.UseAuthentication();           // 3. Auth
app.UseAuthorization();            // 4. Authz
app.MapControllers();              // 5. Routes

app.Run();
```

---

## Execution Plan

### Phase 1: Solution Setup
| Step | Task | Status |
|------|------|--------|
| 1.1 | Create `Boilerplate.sln` | ✅ |
| 1.2 | Create `Boilerplate.Domain.csproj` | ✅ |
| 1.3 | Create `Boilerplate.Application.csproj` → refs Domain | ✅ |
| 1.4 | Create `Boilerplate.Infrastructure.csproj` → refs Application | ✅ |
| 1.5 | Create `Boilerplate.Presentation.csproj` → refs Application, Infrastructure | ✅ |
| 1.6 | Create `Boilerplate.Tests.csproj` | ✅ |

### Phase 2: Domain Layer
| Step | Task | Status |
|------|------|--------|
| 2.1 | `Common/BaseEntity.cs` | ✅ |
| 2.2 | `Common/Interfaces/ISoftDeletable.cs` | ✅ |
| 2.3 | `Common/Interfaces/IAuditable.cs` | ✅ |
| 2.4 | `Entities/User.cs` : ISoftDeletable, IAuditable | ✅ |
| 2.5 | `Entities/Role.cs` : ISoftDeletable | ✅ |
| 2.6 | `Entities/RefreshToken.cs` : BaseEntity only | ✅ |

### Phase 3: Application Layer
| Step | Task | Status |
|------|------|--------|
| 3.1 | `Common/Exceptions/AppException.cs` | ✅ |
| 3.2 | `Common/Exceptions/NotFoundException.cs` | ✅ |
| 3.3 | `Common/Exceptions/ValidationException.cs` | ✅ |
| 3.4 | `Common/Exceptions/ConflictException.cs` | ✅ |
| 3.5 | `Common/Exceptions/UnauthorizedException.cs` | ✅ |
| 3.6 | `Common/Exceptions/ForbiddenException.cs` | ✅ |
| 3.7 | `Common/Abstractions/ICurrentUserService.cs` | ✅ |
| 3.8 | `Common/Settings/JwtSettings.cs` | ✅ |
| 3.9 | `Common/Settings/MongoDbSettings.cs` | ✅ |
| 3.10 | `Interfaces/Services/*` | ✅ |
| 3.11 | `Interfaces/Repositories/*` | ✅ |
| 3.12 | `Interfaces/Infrastructure/*` | ✅ |
| 3.13 | `DTOs/*` | ✅ |
| 3.14 | `Mappings/*` | ✅ |
| 3.15 | `Services/AuthService.cs` | ✅ |
| 3.16 | `Services/UserService.cs` | ✅ |
| 3.17 | `Services/RoleService.cs` | ✅ |
| 3.18 | `DependencyInjection.cs` | ✅ |

### Phase 4: Infrastructure Layer
| Step | Task | Status |
|------|------|--------|
| 4.1 | `Identity/JwtProvider.cs` | ✅ |
| 4.2 | `Persistence/MongoDbContext.cs` | ✅ |
| 4.3 | `Persistence/MongoMappings.cs` | ✅ |
| 4.4 | `Persistence/Common/*` | ✅ |
| 4.5 | `Persistence/Repositories/*` | ✅ |
| 4.6 | `Storage/LocalImageStorageService.cs` | ✅ |
| 4.7 | `DependencyInjection.cs` | ✅ |

### Phase 5: Presentation Layer
| Step | Task | Status |
|------|------|--------|
| 5.1 | `Middleware/ExceptionMiddleware.cs` | ⬜ |
| 5.2 | `Services/CurrentUserService.cs` | ⬜ |
| 5.3 | `Extensions/CorsExtensions.cs` | ⬜ |
| 5.4 | `Extensions/JwtExtensions.cs` | ⬜ |
| 5.5 | `Extensions/SettingsExtensions.cs` | ⬜ |
| 5.6 | `Extensions/SwaggerExtensions.cs` | ⬜ |
| 5.7 | `Controllers/AuthController.cs` | ⬜ |
| 5.8 | `Controllers/UserController.cs` | ⬜ |
| 5.9 | `Controllers/RoleController.cs` | ⬜ |
| 5.10 | `Program.cs` | ⬜ |
| 5.11 | `appsettings.json` | ⬜ |

### Phase 6: Finalize
| Step | Task | Status |
|------|------|--------|
| 6.1 | `.gitignore` | ⬜ |
| 6.2 | `.env.example` | ⬜ |
| 6.3 | `README.md` | ⬜ |
| 6.4 | `dotnet build` ✅ | ⬜ |
| 6.5 | Git commit | ⬜ |

---

## Definition of Done

- [ ] `dotnet build` succeeds
- [ ] Controllers are 3-5 lines each (no branching)
- [ ] Services throw exceptions (never return null)
- [ ] `ExceptionMiddleware` handles all errors
- [ ] No `try-catch` in controllers
- [ ] No `Result<T>` anywhere
- [ ] No `IActionResult` in controller methods
- [ ] `ICurrentUserService` is available in Application layer
- [ ] All namespaces follow `Boilerplate.{Layer}` convention

---

## Response Contract

All API responses follow this shape:

**Success (200-299):**
```json
{
  "success": true,
  "data": { ... }
}
```

**Business Error (400-499):**
```json
{
  "success": false,
  "message": "Email already registered.",
  "errorCode": "Conflict"
}
```

**Server Error (500):**
```json
{
  "success": false,
  "message": "An unexpected error occurred.",
  "errorCode": "ServerError",
  "traceId": "00-abc123..."
}
```

---

## Exception → HTTP Mapping

| Exception | HTTP Status | When to Use |
|-----------|-------------|-------------|
| `NotFoundException` | 404 | Entity not found by ID |
| `ValidationException` | 400 | Business rule violation |
| `ConflictException` | 409 | Duplicate resource |
| `UnauthorizedException` | 401 | Bad credentials |
| `ForbiddenException` | 403 | Valid auth, no permission |
| `Exception` (unhandled) | 500 | Bugs, infra failures |

---

## Why This Works

1. **One error path** — Exceptions only, no dual systems
2. **Middleware owns HTTP** — Controllers don't know status codes
3. **Controllers are dumb** — Call → return (assume success)
4. **Easy to test** — Assert throws, not Result.IsFailure
5. **Fast to write** — No boilerplate per endpoint
6. **Request tracing** — TraceId on 500s for debugging

This is pragmatic. This is opinionated. This is fast.
