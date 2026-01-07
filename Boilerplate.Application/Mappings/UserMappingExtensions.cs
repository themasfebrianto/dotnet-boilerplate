using Boilerplate.Application.DTOs.User;
using Boilerplate.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace Boilerplate.Application.Mappings;

/// <summary>
/// Source-generated mappings for User entity.
/// </summary>
[Mapper]
#pragma warning disable RMG012, RMG020
public static partial class UserMappingExtensions
{
    /// <summary>
    /// Map User entity to UserResponseDto.
    /// </summary>
    [MapProperty(nameof(User.Role) + "." + nameof(Role.Name), nameof(UserResponseDto.RoleName))]
    public static partial UserResponseDto ToDto(this User user);

    /// <summary>
    /// Map list of User entities to list of UserResponseDto.
    /// </summary>
    public static partial List<UserResponseDto> ToDtoList(this List<User> users);

    /// <summary>
    /// Map UserRequestDto to User entity (for creation).
    /// Password hashing is done separately in service.
    /// </summary>
    [MapperIgnoreSource(nameof(UserRequestDto.Password))]
    public static partial User ToEntity(this UserRequestDto dto);

    /// <summary>
    /// Update existing User entity from UserRequestDto.
    /// </summary>
    [MapperIgnoreSource(nameof(UserRequestDto.Password))]
    [MapperIgnoreTarget(nameof(User.Id))]
    [MapperIgnoreTarget(nameof(User.CreatedAt))]
    [MapperIgnoreTarget(nameof(User.PasswordHash))]
    public static partial void UpdateEntity(this UserRequestDto dto, User entity);
}
#pragma warning restore RMG012, RMG020
