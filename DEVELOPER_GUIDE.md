# Developer Guide: Adding Features to the Boilerplate

> **Goal:** Make writing correct code the default. A mid-level developer should be able to add a business feature without guidance.

---

## Table of Contents

1. [Philosophy Recap](#philosophy-recap)
2. [Naming Conventions](#naming-conventions)
3. [Adding a New CRUD Module](#adding-a-new-crud-module)
4. [Adding a New Business Rule](#adding-a-new-business-rule)
5. [Adding a Cross-Cutting Concern](#adding-a-cross-cutting-concern)
6. [Exception Decision Tree](#exception-decision-tree)
7. [Controller Patterns](#controller-patterns)
8. [What NOT to Touch](#what-not-to-touch)
9. [Mental Model](#mental-model)

---

## Philosophy Recap

| Layer | Responsibility |
|-------|---------------|
| **Domain** | Entities + interfaces (`ISoftDeletable`, `IAuditable`) |
| **Application** | Throw intentful exceptions, orchestrate business logic |
| **Infrastructure** | Implement repositories, external services, persistence |
| **Presentation** | Thin controllers (3-5 lines), middleware owns HTTP |

**Rules:**
- Controllers assume success — no `try-catch`, no `if (result == null)`
- Services throw exceptions — never return null for single lookups
- Middleware handles all HTTP mapping — controllers don't know status codes
- One error path — exceptions only, no `Result<T>` pattern

---

## Naming Conventions

| Artifact | Convention | Example |
|----------|------------|---------|
| **Entity** | Singular, PascalCase | `Product`, `Order`, `Customer` |
| **Service Interface** | `I{Entity}Service` | `IProductService` |
| **Service Implementation** | `{Entity}Service` | `ProductService` |
| **Repository Interface** | `I{Entity}Repository` | `IProductRepository` |
| **Repository Implementation** | `{Entity}Repository` | `ProductRepository` |
| **Controller** | `{Entity}Controller` | `ProductController` |
| **Request DTO** | `{Entity}RequestDto` | `ProductRequestDto` |
| **Response DTO** | `{Entity}ResponseDto` | `ProductResponseDto` |
| **Mapping Extensions** | `{Entity}MappingExtensions` | `ProductMappingExtensions` |
| **Exception** | `{Semantics}Exception` | `NotFoundException`, `ValidationException` |

---

## Adding a New CRUD Module

### Step-by-Step Checklist

Use this checklist when adding a new entity (e.g., `Product`):

```
□ 1. Domain/Entities/Product.cs
□ 2. Application/DTOs/Product/ProductRequestDto.cs
□ 3. Application/DTOs/Product/ProductResponseDto.cs
□ 4. Application/Mappings/ProductMappingExtensions.cs
□ 5. Application/Interfaces/Repositories/IProductRepository.cs
□ 6. Application/Interfaces/Services/IProductService.cs
□ 7. Application/Services/ProductService.cs
□ 8. Infrastructure/Persistence/Repositories/ProductRepository.cs
□ 9. Presentation/Controllers/ProductController.cs
□ 10. Application/DependencyInjection.cs → register service
□ 11. Infrastructure/DependencyInjection.cs → register repository
```

### Detailed Walkthrough

#### Step 1: Create Entity

**File:** `Domain/Entities/Product.cs`

```csharp
using Boilerplate.Domain.Common;
using Boilerplate.Domain.Common.Interfaces;

namespace Boilerplate.Domain.Entities;

public class Product : BaseEntity, ISoftDeletable, IAuditable
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }

    // ISoftDeletable
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    // IAuditable
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
}
```

**Why here?** Domain owns the data shape. Entities are persistence-agnostic.

---

#### Step 2-3: Create DTOs

**File:** `Application/DTOs/Product/ProductRequestDto.cs`

```csharp
namespace Boilerplate.Application.DTOs.Product;

public class ProductRequestDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
}
```

**File:** `Application/DTOs/Product/ProductResponseDto.cs`

> **Note:** Implement `IHasId` to enable the simplified `Created()` helper in controllers.

```csharp
using Boilerplate.Application.Common.Abstractions;

namespace Boilerplate.Application.DTOs.Product;

public class ProductResponseDto : IHasId
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

**Why here?** DTOs define the API contract. They live in Application because services consume them.

---

#### Step 4: Create Mapping Extensions

**File:** `Application/Mappings/ProductMappingExtensions.cs`

```csharp
using Boilerplate.Application.DTOs.Product;
using Boilerplate.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace Boilerplate.Application.Mappings;

[Mapper]
public static partial class ProductMappingExtensions
{
    public static partial ProductResponseDto ToDto(this Product product);
    public static partial List<ProductResponseDto> ToDtoList(this List<Product> products);
    
    [MapperIgnoreSource(nameof(ProductRequestDto.Price))] // Example: handle manually if needed
    public static partial Product ToEntity(this ProductRequestDto dto);
    
    [MapperIgnoreTarget(nameof(Product.Id))]
    [MapperIgnoreTarget(nameof(Product.CreatedAt))]
    public static partial void UpdateEntity(this ProductRequestDto dto, Product entity);
}
```

**Why here?** Mapperly generates code at compile-time. Application owns the mapping logic.

---

#### Step 5: Create Repository Interface

**File:** `Application/Interfaces/Repositories/IProductRepository.cs`

```csharp
using Boilerplate.Domain.Entities;

namespace Boilerplate.Application.Interfaces.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id);
    Task<List<Product>> GetAllAsync();
    Task<Product> CreateAsync(Product product);
    Task<Product> UpdateAsync(Product product);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    
    // Entity-specific methods
    Task<List<Product>> GetByPriceRangeAsync(decimal min, decimal max);
}
```

**Why here?** Application defines the contract. Infrastructure implements it.

---

#### Step 6: Create Service Interface

**File:** `Application/Interfaces/Services/IProductService.cs`

```csharp
using Boilerplate.Application.DTOs.Product;

namespace Boilerplate.Application.Interfaces.Services;

/// <summary>
/// Product service interface. All methods throw exceptions on failure.
/// </summary>
public interface IProductService
{
    /// <exception cref="Common.Exceptions.NotFoundException">Product not found.</exception>
    Task<ProductResponseDto> GetByIdAsync(Guid id);
    
    Task<List<ProductResponseDto>> GetAllAsync();
    
    /// <exception cref="Common.Exceptions.ValidationException">Invalid product data.</exception>
    Task<ProductResponseDto> CreateAsync(ProductRequestDto request);
    
    /// <exception cref="Common.Exceptions.NotFoundException">Product not found.</exception>
    Task<ProductResponseDto> UpdateAsync(Guid id, ProductRequestDto request);
    
    /// <exception cref="Common.Exceptions.NotFoundException">Product not found.</exception>
    Task DeleteAsync(Guid id);
    
    /// <exception cref="Common.Exceptions.NotFoundException">Product not found.</exception>
    Task SoftDeleteAsync(Guid id);
}
```

**Why here?** Document exceptions in XML comments. This is the service contract.

---

#### Step 7: Implement Service

**File:** `Application/Services/ProductService.cs`

```csharp
using Boilerplate.Application.Common.Abstractions;
using Boilerplate.Application.Common.Exceptions;
using Boilerplate.Application.DTOs.Product;
using Boilerplate.Application.Interfaces.Repositories;
using Boilerplate.Application.Interfaces.Services;
using Boilerplate.Application.Mappings;

namespace Boilerplate.Application.Services;

public class ProductService(
    IProductRepository productRepository,
    ICurrentUserService currentUserService) : IProductService
{
    public async Task<ProductResponseDto> GetByIdAsync(Guid id)
    {
        var product = await productRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Product", id);
        return product.ToDto();
    }

    public async Task<List<ProductResponseDto>> GetAllAsync()
    {
        var products = await productRepository.GetAllAsync();
        return products.ToDtoList();
    }

    public async Task<ProductResponseDto> CreateAsync(ProductRequestDto request)
    {
        // Business rule: Price must be positive
        if (request.Price <= 0)
            throw new ValidationException("Product price must be greater than zero.");

        var product = request.ToEntity();
        product.CreatedAt = DateTime.UtcNow;
        product.CreatedBy = currentUserService.UserId;

        var created = await productRepository.CreateAsync(product);
        return created.ToDto();
    }

    public async Task<ProductResponseDto> UpdateAsync(Guid id, ProductRequestDto request)
    {
        var product = await productRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Product", id);

        if (request.Price <= 0)
            throw new ValidationException("Product price must be greater than zero.");

        request.UpdateEntity(product);
        product.UpdatedAt = DateTime.UtcNow;
        product.UpdatedBy = currentUserService.UserId;

        var updated = await productRepository.UpdateAsync(product);
        return updated.ToDto();
    }

    public async Task DeleteAsync(Guid id)
    {
        if (!await productRepository.ExistsAsync(id))
            throw new NotFoundException("Product", id);

        await productRepository.DeleteAsync(id);
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        var product = await productRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Product", id);

        product.DeletedAt = DateTime.UtcNow;
        product.DeletedBy = currentUserService.UserId;

        await productRepository.UpdateAsync(product);
    }
}
```

**Why here?** All business logic lives in services. Throw exceptions, never return null.

---

#### Step 8: Implement Repository

**File:** `Infrastructure/Persistence/Repositories/ProductRepository.cs`

> **Note:** Extend `MongoRepositoryBase<T>` to get standard CRUD operations for free.
> Only implement entity-specific methods.

```csharp
using Boilerplate.Application.Interfaces.Repositories;
using Boilerplate.Domain.Entities;
using Boilerplate.Infrastructure.Persistence.Repositories.Common;
using MongoDB.Driver;

namespace Boilerplate.Infrastructure.Persistence.Repositories;

/// <summary>
/// Product repository implementation.
/// Extends MongoRepositoryBase for standard CRUD, only implements entity-specific methods.
/// </summary>
public class ProductRepository(IMongoDbContext context) 
    : MongoRepositoryBase<Product>(context), IProductRepository
{
    /// <summary>
    /// Get products within a price range.
    /// </summary>
    public async Task<List<Product>> GetByPriceRangeAsync(decimal min, decimal max)
    {
        var filter = Builders<Product>.Filter.Gte(x => x.Price, min) &
                     Builders<Product>.Filter.Lte(x => x.Price, max);
        return await FindAsync(filter);
    }
}
```

**Why here?** Infrastructure owns persistence. `MongoRepositoryBase` provides:
- `GetByIdAsync(Guid id)` — with soft-delete filter
- `GetAllAsync()` — with soft-delete filter  
- `CreateAsync(T entity)` — sets Id and CreatedAt
- `UpdateAsync(T entity)` — sets UpdatedAt
- `DeleteAsync(Guid id)` — hard delete
- `ExistsAsync(Guid id)` — with soft-delete filter
- `FindAsync(filter)` — protected helper for custom queries
- `FindOneAsync(filter)` — protected helper for single-entity queries
- `AnyAsync(filter)` — protected helper for existence checks

---

#### Step 9: Create Controller

**File:** `Presentation/Controllers/ProductController.cs`

> **Note:** Extend `ApiController` to get the `Created()` helper method.
> Response DTOs must implement `IHasId` to use the simplified `Created(entity)` overload.

```csharp
using Boilerplate.Application.DTOs.Product;
using Boilerplate.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boilerplate.Presentation.Controllers;

/// <summary>
/// Product management endpoints.
/// Controllers are thin - just call service and return.
/// </summary>
[Route("api/[controller]")]
[Authorize]
public class ProductController(IProductService productService) : ApiController
{
    [HttpGet]
    public async Task<List<ProductResponseDto>> GetAll()
        => await productService.GetAllAsync();

    [HttpGet("{id:guid}")]
    public async Task<ProductResponseDto> GetById(Guid id)
        => await productService.GetByIdAsync(id);

    [HttpPost]
    public async Task<ActionResult<ProductResponseDto>> Create([FromBody] ProductRequestDto request)
    {
        var product = await productService.CreateAsync(request);
        return Created(product);  // Uses IHasId - no need for CreatedAtAction ceremony
    }

    [HttpPut("{id:guid}")]
    public async Task<ProductResponseDto> Update(Guid id, [FromBody] ProductRequestDto request)
        => await productService.UpdateAsync(id, request);

    [HttpDelete("{id:guid}")]
    public async Task Delete(Guid id)
        => await productService.DeleteAsync(id);

    [HttpPost("{id:guid}/soft-delete")]
    public async Task SoftDelete(Guid id)
        => await productService.SoftDeleteAsync(id);
}
```

**Why here?** Controllers are thin. Call service → return. No business logic.

---

#### Step 10-11: Register in DI

**File:** `Application/DependencyInjection.cs` — add:

```csharp
services.AddScoped<IProductService, ProductService>();
```

**File:** `Infrastructure/DependencyInjection.cs` — add:

```csharp
services.AddScoped<IProductRepository, ProductRepository>();
```

---

## Adding a New Business Rule

Business rules live in **services**. Use exceptions to enforce them.

### Example: "Product stock cannot go negative"

```csharp
// In ProductService
public async Task DeductStock(Guid id, int quantity)
{
    var product = await productRepository.GetByIdAsync(id)
        ?? throw new NotFoundException("Product", id);

    if (product.Stock < quantity)
        throw new ValidationException($"Insufficient stock. Available: {product.Stock}, Requested: {quantity}");

    product.Stock -= quantity;
    product.UpdatedAt = DateTime.UtcNow;
    product.UpdatedBy = currentUserService.UserId;

    await productRepository.UpdateAsync(product);
}
```

### Rules for Business Logic

| Rule | Implementation |
|------|----------------|
| Single entity validation | Throw in service method |
| Cross-entity validation | Inject multiple repositories, validate in service |
| Complex domain logic | Consider a domain service, but keep it in Application layer |

---

## Adding a Cross-Cutting Concern

| Concern | Where to Add | Example |
|---------|--------------|---------|
| **New Configuration** | `Application/Common/Settings/{Name}Settings.cs` | `EmailSettings.cs` |
| **New External Service** | Interface: `Application/Interfaces/Infrastructure/I{Name}Service.cs`<br>Implementation: `Infrastructure/{Category}/{Name}Service.cs` | `IEmailService`, `SendGridEmailService` |
| **New Middleware** | `Presentation/Middleware/{Name}Middleware.cs` | `RateLimitMiddleware.cs` |
| **New Exception Type** | `Application/Common/Exceptions/{Name}Exception.cs` | `RateLimitedException.cs` (429) |

### Example: Adding an Email Service

```
□ Application/Common/Settings/EmailSettings.cs
□ Application/Interfaces/Infrastructure/IEmailService.cs
□ Infrastructure/Email/SendGridEmailService.cs
□ Infrastructure/DependencyInjection.cs → register service
□ appsettings.json → add EmailSettings section
```

---

## Exception Decision Tree

Use this flowchart to decide which exception to throw:

```
┌─────────────────────────────────────────────────────────────────┐
│                    EXCEPTION DECISION TREE                      │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
                    ┌─────────────────┐
                    │ Data not found? │
                    └────────┬────────┘
                             │
              ┌──────────────┼──────────────┐
              │ YES                         │ NO
              ▼                             ▼
    ┌─────────────────┐          ┌─────────────────────┐
    │ NotFoundException│          │ Business rule       │
    │ ("Entity", id)   │          │ violated?           │
    └─────────────────┘          └──────────┬──────────┘
                                            │
                              ┌─────────────┼─────────────┐
                              │ YES                       │ NO
                              ▼                           ▼
                    ┌─────────────────┐        ┌─────────────────────┐
                    │ ValidationException│      │ Duplicate resource? │
                    │ ("message")       │      └──────────┬──────────┘
                    └─────────────────┘                   │
                                            ┌─────────────┼─────────────┐
                                            │ YES                       │ NO
                                            ▼                           ▼
                                  ┌─────────────────┐        ┌─────────────────────┐
                                  │ ConflictException│        │ Authentication      │
                                  │ ("message")      │        │ failed?             │
                                  └─────────────────┘        └──────────┬──────────┘
                                                                        │
                                                          ┌─────────────┼─────────────┐
                                                          │ YES                       │ NO
                                                          ▼                           ▼
                                                ┌───────────────────┐      ┌─────────────────────┐
                                                │UnauthorizedException│     │ Authorized but no   │
                                                │ ("message")        │     │ permission?         │
                                                └───────────────────┘     └──────────┬──────────┘
                                                                                      │
                                                                        ┌─────────────┼─────────────┐
                                                                        │ YES                       │ NO
                                                                        ▼                           ▼
                                                              ┌─────────────────┐      ┌─────────────────┐
                                                              │ForbiddenException│      │ New HTTP status │
                                                              │ ("message")      │      │ needed? Create  │
                                                              └─────────────────┘      │ new exception.  │
                                                                                       └─────────────────┘
```

### Exception → HTTP Status Code Mapping

| Exception | HTTP Status | When to Use |
|-----------|-------------|-------------|
| `NotFoundException` | 404 | Entity not found by ID |
| `ValidationException` | 400 | Business rule violation, invalid input |
| `ConflictException` | 409 | Duplicate resource, optimistic concurrency |
| `UnauthorizedException` | 401 | Bad credentials, expired token |
| `ForbiddenException` | 403 | Valid auth, but no permission |
| `Exception` (unhandled) | 500 | Bugs, infrastructure failures |

### When to Create a New Exception

**Only create a new exception if:**
1. You need a **different HTTP status code** (e.g., 429 for rate limiting)
2. The exception has **additional structured data** beyond a message

**Don't create:**
- `ProductNotFoundException` (use `NotFoundException("Product", id)`)
- `InvalidPriceException` (use `ValidationException("Price must be positive")`)

---

## Controller Patterns

### Pattern 1: Return Data (200 OK)

```csharp
[HttpGet("{id:guid}")]
public async Task<ProductResponseDto> GetById(Guid id)
    => await productService.GetByIdAsync(id);
```

### Pattern 2: Create and Return (201 Created)

```csharp
// If ResponseDto implements IHasId (preferred)
[HttpPost]
public async Task<ActionResult<ProductResponseDto>> Create([FromBody] ProductRequestDto request)
{
    var product = await productService.CreateAsync(request);
    return Created(product);  // Clean one-liner!
}

// Alternative: explicit ID selector
[HttpPost]
public async Task<ActionResult<ProductResponseDto>> Create([FromBody] ProductRequestDto request)
{
    var product = await productService.CreateAsync(request);
    return Created(product, p => p.Id);
}
```

### Pattern 3: Action with No Return (204 No Content)

```csharp
[HttpDelete("{id:guid}")]
public async Task Delete(Guid id)
    => await productService.DeleteAsync(id);
```

### Anti-Patterns to Avoid

```csharp
// ❌ DON'T: Try-catch in controller
[HttpGet("{id:guid}")]
public async Task<IActionResult> GetById(Guid id)
{
    try
    {
        var product = await productService.GetByIdAsync(id);
        return Ok(product);
    }
    catch (NotFoundException)
    {
        return NotFound();
    }
}

// ❌ DON'T: Null checking in controller
[HttpGet("{id:guid}")]
public async Task<IActionResult> GetById(Guid id)
{
    var product = await productService.GetByIdAsync(id);
    if (product == null)
        return NotFound();
    return Ok(product);
}

// ❌ DON'T: Business logic in controller
[HttpPost]
public async Task<ActionResult<ProductResponseDto>> Create([FromBody] ProductRequestDto request)
{
    if (request.Price <= 0)  // This belongs in service!
        return BadRequest("Price must be positive");
    
    var product = await productService.CreateAsync(request);
    return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
}
```

---

## What NOT to Touch

These files should rarely need modification:

| File | Why Not Touch |
|------|---------------|
| `ExceptionMiddleware.cs` | Generic exception handling — already covers all `AppException` types |
| `Program.cs` | Layer registration handles new modules automatically |
| `appsettings.json` | Only modify when adding **new configuration sections** |
| `BaseEntity.cs` | Core entity contract — modifying affects all entities |
| `AppException.cs` | Base exception contract — derivatives inherit from it |

---

## Mental Model

```
┌─────────────────────────────────────────────────────────────────────────────┐
│  DEVELOPER MENTAL MODEL                                                     │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  1. "Where does business logic go?"                                         │
│     → Application/Services/{Entity}Service.cs                               │
│                                                                             │
│  2. "Where does data access go?"                                            │
│     → Infrastructure/Persistence/Repositories/{Entity}Repository.cs         │
│                                                                             │
│  3. "Where do HTTP concerns go?"                                            │
│     → Presentation/Controllers/{Entity}Controller.cs                        │
│     → Presentation/Middleware/ (for cross-cutting)                          │
│                                                                             │
│  4. "How do I handle errors?"                                               │
│     → Throw the appropriate AppException from service                       │
│     → Middleware maps it to HTTP automatically                              │
│                                                                             │
│  5. "How do I add a new module?"                                            │
│     → Follow the 11-file checklist                                          │
│     → Start from Domain, work outward                                       │
│                                                                             │
│  6. "How do I add a new external service?"                                  │
│     → Interface in Application/Interfaces/Infrastructure/                   │
│     → Implementation in Infrastructure/{Category}/                          │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## Quick Reference Card

### Adding a CRUD Module

```
Domain/Entities/{Entity}.cs
Application/DTOs/{Entity}/{Entity}RequestDto.cs
Application/DTOs/{Entity}/{Entity}ResponseDto.cs
Application/Mappings/{Entity}MappingExtensions.cs
Application/Interfaces/Repositories/I{Entity}Repository.cs
Application/Interfaces/Services/I{Entity}Service.cs
Application/Services/{Entity}Service.cs
Infrastructure/Persistence/Repositories/{Entity}Repository.cs
Presentation/Controllers/{Entity}Controller.cs
+ Register in Application/DependencyInjection.cs
+ Register in Infrastructure/DependencyInjection.cs
```

### Exception Quick Reference

| Situation | Exception |
|-----------|-----------|
| Not found | `throw new NotFoundException("Entity", id);` |
| Invalid input | `throw new ValidationException("Message");` |
| Duplicate | `throw new ConflictException("Entity", key);` |
| Bad credentials | `throw new UnauthorizedException();` |
| No permission | `throw new ForbiddenException("Message");` |

### Controller Quick Reference

```csharp
// GET single → Task<T>
[HttpGet("{id:guid}")]
public async Task<T> GetById(Guid id) => await service.GetByIdAsync(id);

// GET list → Task<List<T>>
[HttpGet]
public async Task<List<T>> GetAll() => await service.GetAllAsync();

// POST → ActionResult<T> with Created() helper (requires ResponseDto : IHasId)
[HttpPost]
public async Task<ActionResult<T>> Create([FromBody] Dto request)
{
    var entity = await service.CreateAsync(request);
    return Created(entity);  // Inherit from ApiController to get this helper
}

// PUT → Task<T>
[HttpPut("{id:guid}")]
public async Task<T> Update(Guid id, [FromBody] Dto request) => await service.UpdateAsync(id, request);

// DELETE → Task
[HttpDelete("{id:guid}")]
public async Task Delete(Guid id) => await service.DeleteAsync(id);
```

---

*This boilerplate is pragmatic. This boilerplate is opinionated. This boilerplate is fast.*
