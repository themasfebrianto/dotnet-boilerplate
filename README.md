# .NET Clean Architecture Boilerplate

A pragmatic, exception-driven .NET 8 boilerplate following Clean Architecture principles.

## 🎯 Philosophy

**One rule. No exceptions (pun intended).**

| Layer | Responsibility |
|-------|---------------|
| **Application** | Throw intentful exceptions |
| **Controller** | Call service → return success (3-5 lines, always) |
| **Middleware** | Catch exception → map to HTTP |
| **Response** | Wrapped centrally (never in controllers) |

## ❌ What We DON'T Have

- ❌ `Result<T>` pattern
- ❌ Controller branching on success/failure
- ❌ Dual error systems
- ❌ Manual response wrapping in controllers
- ❌ MediatR/CQRS ceremony

## ✅ What We DO Have

- ✅ Typed exceptions with semantic meaning
- ✅ One global middleware that handles ALL HTTP mapping
- ✅ Controllers that assume success
- ✅ Automatic response wrapping
- ✅ 3-5 line controller methods

## 📁 Project Structure

```
Boilerplate/
├── Boilerplate.Domain/           # Entities, Interfaces (ISoftDeletable, IAuditable)
├── Boilerplate.Application/      # Services, DTOs, Exceptions, Interfaces
├── Boilerplate.Infrastructure/   # MongoDB, JWT Provider, Repositories
├── Boilerplate.Presentation/     # Controllers, Middleware, Extensions
└── Boilerplate.Tests/            # Unit tests
```

## 🚀 Quick Start

### Prerequisites

- .NET 8 SDK
- MongoDB (local or Docker)

### Setup

```bash
# Clone the repository
git clone https://github.com/themasfebrianto/dotnet-boilerplate.git
cd dotnet-boilerplate

# Copy environment configuration
cp .env.example .env

# Restore packages
dotnet restore

# Run the application
dotnet run --project Boilerplate.Presentation
```

### Access

- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger

## 📖 Exception → HTTP Mapping

| Exception | HTTP Status | When to Use |
|-----------|-------------|-------------|
| `NotFoundException` | 404 | Entity not found by ID |
| `ValidationException` | 400 | Business rule violation |
| `ConflictException` | 409 | Duplicate resource |
| `UnauthorizedException` | 401 | Bad credentials |
| `ForbiddenException` | 403 | Valid auth, no permission |
| `Exception` (unhandled) | 500 | Bugs, infra failures |

## 📝 Response Contract

### Success (200-299)
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "abc123...",
  "accessTokenExpires": "2024-01-01T12:15:00Z",
  "refreshTokenExpires": "2024-01-08T12:00:00Z"
}
```

### Business Error (400-499)
```json
{
  "success": false,
  "message": "Email already registered.",
  "errorCode": "Conflict"
}
```

### Server Error (500)
```json
{
  "success": false,
  "message": "An unexpected error occurred.",
  "errorCode": "ServerError",
  "traceId": "00-abc123..."
}
```

## 🔧 Configuration

### appsettings.json

```json
{
  "JwtSettings": {
    "IssuerSigningKey": "your-secret-key-32-chars-minimum",
    "ValidIssuer": "Boilerplate",
    "ValidAudience": "Boilerplate",
    "Expiration": "00:15:00",
    "RefreshTokenExpiration": "7.00:00:00"
  },
  "MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "boilerplate"
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:3000"]
  }
}
```

## 🏗️ Architecture Highlights

### Domain Layer
- **Interface-based composition** instead of deep inheritance
- `ISoftDeletable` and `IAuditable` interfaces
- Entities explicitly declare their capabilities

### Application Layer
- **Exception-driven flow** - services throw, never return null
- `ICurrentUserService` for cross-cutting user context
- Mapperly for compile-time generated mappings

### Infrastructure Layer
- MongoDB with soft-delete aware query filters
- Thread-safe BSON mappings registration
- JWT provider with configurable settings

### Presentation Layer
- **ExceptionMiddleware** - the heart of error handling
- Clean controllers (3-5 lines per method)
- Swagger with JWT authentication support

## 📚 API Endpoints

### Auth
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration
- `POST /api/auth/refresh` - Refresh access token
- `POST /api/auth/revoke` - Revoke refresh token

### Users (requires authentication)
- `GET /api/user` - Get all users
- `GET /api/user/{id}` - Get user by ID
- `POST /api/user` - Create user
- `PUT /api/user/{id}` - Update user
- `DELETE /api/user/{id}` - Delete user
- `POST /api/user/{id}/change-password` - Change password
- `POST /api/user/{id}/soft-delete` - Soft delete user
- `POST /api/user/{userId}/role/{roleId}` - Assign role

### Roles (requires authentication)
- `GET /api/role` - Get all roles
- `GET /api/role/{id}` - Get role by ID
- `POST /api/role` - Create role
- `PUT /api/role/{id}` - Update role
- `DELETE /api/role/{id}` - Delete role

## 🧪 Testing

```bash
dotnet test
```

## 📄 License

MIT License

---

**This is pragmatic. This is opinionated. This is fast.**
