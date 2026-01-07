using Boilerplate.Application.DTOs.Role;
using Boilerplate.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boilerplate.Presentation.Controllers;

/// <summary>
/// Role management endpoints.
/// Controllers are thin - just call service and return.
/// </summary>
[Route("api/[controller]")]
[Authorize]
public class RoleController(IRoleService roleService) : ApiController
{
    [HttpGet]
    public async Task<List<RoleResponseDto>> GetAll()
        => await roleService.GetAllAsync();

    [HttpGet("{id:guid}")]
    public async Task<RoleResponseDto> GetById(Guid id)
        => await roleService.GetByIdAsync(id);

    [HttpGet("name/{name}")]
    public async Task<RoleResponseDto> GetByName(string name)
        => await roleService.GetByNameAsync(name);

    [HttpPost]
    public async Task<ActionResult<RoleResponseDto>> Create([FromBody] RoleRequestDto request)
    {
        var role = await roleService.CreateAsync(request);
        return Created(role);
    }

    [HttpPut("{id:guid}")]
    public async Task<RoleResponseDto> Update(Guid id, [FromBody] RoleRequestDto request)
        => await roleService.UpdateAsync(id, request);

    [HttpDelete("{id:guid}")]
    public async Task Delete(Guid id)
        => await roleService.DeleteAsync(id);
}
