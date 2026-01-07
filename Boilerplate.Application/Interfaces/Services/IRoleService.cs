using Boilerplate.Application.DTOs.Role;

namespace Boilerplate.Application.Interfaces.Services;

/// <summary>
/// Role management service interface.
/// All methods throw exceptions on failure (exception-driven flow).
/// </summary>
public interface IRoleService
{
    /// <exception cref="Common.Exceptions.NotFoundException">Role not found.</exception>
    Task<RoleResponseDto> GetByIdAsync(Guid id);

    /// <exception cref="Common.Exceptions.NotFoundException">Role not found.</exception>
    Task<RoleResponseDto> GetByNameAsync(string name);

    Task<List<RoleResponseDto>> GetAllAsync();

    /// <exception cref="Common.Exceptions.ConflictException">Role name already exists.</exception>
    Task<RoleResponseDto> CreateAsync(RoleRequestDto request);

    /// <exception cref="Common.Exceptions.NotFoundException">Role not found.</exception>
    Task<RoleResponseDto> UpdateAsync(Guid id, RoleRequestDto request);

    /// <exception cref="Common.Exceptions.NotFoundException">Role not found.</exception>
    Task DeleteAsync(Guid id);
}
