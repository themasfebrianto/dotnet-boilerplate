using Boilerplate.Application.DTOs.Role;
using Boilerplate.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace Boilerplate.Application.Mappings;

/// <summary>
/// Source-generated mappings for Role entity.
/// </summary>
[Mapper]
#pragma warning disable RMG012, RMG020
public static partial class RoleMappingExtensions
{
    /// <summary>
    /// Map Role entity to RoleResponseDto.
    /// </summary>
    public static partial RoleResponseDto ToDto(this Role role);

    /// <summary>
    /// Map list of Role entities to list of RoleResponseDto.
    /// </summary>
    public static partial List<RoleResponseDto> ToDtoList(this List<Role> roles);

    /// <summary>
    /// Map RoleRequestDto to Role entity (for creation).
    /// </summary>
    public static partial Role ToEntity(this RoleRequestDto dto);

    /// <summary>
    /// Update existing Role entity from RoleRequestDto.
    /// </summary>
    [MapperIgnoreTarget(nameof(Role.Id))]
    [MapperIgnoreTarget(nameof(Role.CreatedAt))]
    public static partial void UpdateEntity(this RoleRequestDto dto, Role entity);
}
#pragma warning restore RMG012, RMG020
