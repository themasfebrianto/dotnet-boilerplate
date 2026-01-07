using Boilerplate.Application.DTOs.User;
using Boilerplate.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boilerplate.Presentation.Controllers;

/// <summary>
/// User management endpoints.
/// Controllers are thin - just call service and return.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet]
    public async Task<List<UserResponseDto>> GetAll()
        => await userService.GetAllAsync();

    [HttpGet("{id:guid}")]
    public async Task<UserResponseDto> GetById(Guid id)
        => await userService.GetByIdAsync(id);

    [HttpGet("email/{email}")]
    public async Task<UserResponseDto> GetByEmail(string email)
        => await userService.GetByEmailAsync(email);

    [HttpPost]
    public async Task<ActionResult<UserResponseDto>> Create([FromBody] UserRequestDto request)
    {
        var user = await userService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPut("{id:guid}")]
    public async Task<UserResponseDto> Update(Guid id, [FromBody] UserRequestDto request)
        => await userService.UpdateAsync(id, request);

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await userService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id:guid}/change-password")]
    public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordRequestDto request)
    {
        await userService.ChangePasswordAsync(id, request);
        return NoContent();
    }

    [HttpPost("{id:guid}/soft-delete")]
    public async Task<IActionResult> SoftDelete(Guid id)
    {
        await userService.SoftDeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{userId:guid}/role/{roleId:guid}")]
    public async Task<IActionResult> SetRole(Guid userId, Guid roleId)
    {
        await userService.SetRoleAsync(userId, roleId);
        return NoContent();
    }
}
