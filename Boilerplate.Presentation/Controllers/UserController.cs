using Boilerplate.Application.DTOs.User;
using Boilerplate.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boilerplate.Presentation.Controllers;

/// <summary>
/// User management endpoints.
/// Controllers are thin - just call service and return.
/// </summary>
[Route("api/[controller]")]
[Authorize]
public class UserController(IUserService userService) : ApiController
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
        return Created(user);
    }

    [HttpPut("{id:guid}")]
    public async Task<UserResponseDto> Update(Guid id, [FromBody] UserRequestDto request)
        => await userService.UpdateAsync(id, request);

    [HttpDelete("{id:guid}")]
    public async Task Delete(Guid id)
        => await userService.DeleteAsync(id);

    [HttpPost("{id:guid}/change-password")]
    public async Task ChangePassword(Guid id, [FromBody] ChangePasswordRequestDto request)
        => await userService.ChangePasswordAsync(id, request);

    [HttpPost("{userId:guid}/role/{roleId:guid}")]
    public async Task SetRole(Guid userId, Guid roleId)
        => await userService.SetRoleAsync(userId, roleId);
}
