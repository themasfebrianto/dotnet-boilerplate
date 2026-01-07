using Boilerplate.Application.DTOs.User;

namespace Boilerplate.Application.Interfaces.Services;

/// <summary>
/// User management service interface.
/// All methods throw exceptions on failure (exception-driven flow).
/// </summary>
public interface IUserService
{
    /// <exception cref="Common.Exceptions.NotFoundException">User not found.</exception>
    Task<UserResponseDto> GetByIdAsync(Guid id);

    /// <exception cref="Common.Exceptions.NotFoundException">User not found.</exception>
    Task<UserResponseDto> GetByEmailAsync(string email);

    Task<List<UserResponseDto>> GetAllAsync();

    /// <exception cref="Common.Exceptions.ConflictException">Email already exists.</exception>
    Task<UserResponseDto> CreateAsync(UserRequestDto request);

    /// <exception cref="Common.Exceptions.NotFoundException">User not found.</exception>
    Task<UserResponseDto> UpdateAsync(Guid id, UserRequestDto request);

    /// <exception cref="Common.Exceptions.NotFoundException">User not found.</exception>
    Task DeleteAsync(Guid id);

    /// <exception cref="Common.Exceptions.NotFoundException">User not found.</exception>
    /// <exception cref="Common.Exceptions.ValidationException">Invalid current password.</exception>
    Task ChangePasswordAsync(Guid id, ChangePasswordRequestDto request);

    /// <exception cref="Common.Exceptions.NotFoundException">User not found.</exception>
    Task SoftDeleteAsync(Guid id);

    /// <exception cref="Common.Exceptions.NotFoundException">User or role not found.</exception>
    Task SetRoleAsync(Guid userId, Guid roleId);
}
