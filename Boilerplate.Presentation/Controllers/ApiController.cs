using Boilerplate.Application.Common.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Boilerplate.Presentation.Controllers;

/// <summary>
/// Base controller providing common helpers for API endpoints.
/// All API controllers should inherit from this class.
/// </summary>
[ApiController]
public abstract class ApiController : ControllerBase
{
    /// <summary>
    /// Creates a 201 Created response with location header pointing to GetById action.
    /// </summary>
    /// <typeparam name="T">The type of entity being returned.</typeparam>
    /// <param name="entity">The created entity to return.</param>
    /// <param name="idSelector">Function to extract the ID from the entity.</param>
    /// <param name="getByIdAction">Name of the GetById action (defaults to "GetById").</param>
    /// <returns>CreatedAtActionResult with the entity and location header.</returns>
    /// <example>
    /// [HttpPost]
    /// public async Task&lt;ActionResult&lt;ProductDto&gt;&gt; Create([FromBody] CreateProductDto request)
    /// {
    ///     var product = await _productService.CreateAsync(request);
    ///     return Created(product, p =&gt; p.Id);
    /// }
    /// </example>
    protected CreatedAtActionResult Created<T>(T entity, Func<T, Guid> idSelector, string getByIdAction = "GetById")
    {
        var id = idSelector(entity);
        return CreatedAtAction(getByIdAction, new { id }, entity);
    }

    /// <summary>
    /// Creates a 201 Created response for entities with a direct Id property.
    /// </summary>
    /// <typeparam name="T">Response DTO type implementing IHasId.</typeparam>
    /// <param name="entity">The created entity to return.</param>
    /// <param name="getByIdAction">Name of the GetById action (defaults to "GetById").</param>
    /// <returns>CreatedAtActionResult with the entity and location header.</returns>
    protected CreatedAtActionResult Created<T>(T entity, string getByIdAction = "GetById") where T : IHasId
    {
        return CreatedAtAction(getByIdAction, new { id = entity.Id }, entity);
    }
}

