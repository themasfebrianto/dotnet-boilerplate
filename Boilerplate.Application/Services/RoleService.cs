using Boilerplate.Application.Common.Exceptions;
using Boilerplate.Application.DTOs.Role;
using Boilerplate.Application.Interfaces.Repositories;
using Boilerplate.Application.Interfaces.Services;
using Boilerplate.Application.Mappings;

namespace Boilerplate.Application.Services;

/// <summary>
/// Role management service implementation.
/// Follows exception-driven flow - throws on failure, never returns null.
/// </summary>
public class RoleService(IRoleRepository roleRepository) : IRoleService
{
    public async Task<RoleResponseDto> GetByIdAsync(Guid id)
    {
        var role = await roleRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Role", id);
        return role.ToDto();
    }

    public async Task<RoleResponseDto> GetByNameAsync(string name)
    {
        var role = await roleRepository.GetByNameAsync(name)
            ?? throw new NotFoundException($"Role '{name}' was not found.");
        return role.ToDto();
    }

    public async Task<List<RoleResponseDto>> GetAllAsync()
    {
        var roles = await roleRepository.GetAllAsync();
        return roles.ToDtoList();
    }

    public async Task<RoleResponseDto> CreateAsync(RoleRequestDto request)
    {
        if (await roleRepository.NameExistsAsync(request.Name))
            throw new ConflictException("Role", request.Name);

        var role = request.ToEntity();
        role.CreatedAt = DateTime.UtcNow;

        var createdRole = await roleRepository.CreateAsync(role);
        return createdRole.ToDto();
    }

    public async Task<RoleResponseDto> UpdateAsync(Guid id, RoleRequestDto request)
    {
        var role = await roleRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Role", id);

        // Check name uniqueness if name is changing
        if (!role.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase))
        {
            if (await roleRepository.NameExistsAsync(request.Name))
                throw new ConflictException("Role", request.Name);
        }

        request.UpdateEntity(role);
        role.UpdatedAt = DateTime.UtcNow;

        var updatedRole = await roleRepository.UpdateAsync(role);
        return updatedRole.ToDto();
    }

    public async Task DeleteAsync(Guid id)
    {
        if (!await roleRepository.ExistsAsync(id))
            throw new NotFoundException("Role", id);

        await roleRepository.DeleteAsync(id);
    }
}
