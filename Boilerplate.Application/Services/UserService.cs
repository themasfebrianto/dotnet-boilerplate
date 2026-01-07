using Boilerplate.Application.Common.Abstractions;
using Boilerplate.Application.Common.Exceptions;
using Boilerplate.Application.DTOs.User;
using Boilerplate.Application.Interfaces.Repositories;
using Boilerplate.Application.Interfaces.Services;
using Boilerplate.Application.Mappings;

namespace Boilerplate.Application.Services;

/// <summary>
/// User management service implementation.
/// Follows exception-driven flow - throws on failure, never returns null.
/// </summary>
public class UserService(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    ICurrentUserService currentUserService) : IUserService
{
    public async Task<UserResponseDto> GetByIdAsync(Guid id)
    {
        var user = await userRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("User", id);
        return user.ToDto();
    }

    public async Task<UserResponseDto> GetByEmailAsync(string email)
    {
        var user = await userRepository.GetByEmailAsync(email)
            ?? throw new NotFoundException($"User with email '{email}' was not found.");
        return user.ToDto();
    }

    public async Task<List<UserResponseDto>> GetAllAsync()
    {
        var users = await userRepository.GetAllAsync();
        return users.ToDtoList();
    }

    public async Task<UserResponseDto> CreateAsync(UserRequestDto request)
    {
        if (await userRepository.EmailExistsAsync(request.Email))
            throw new ConflictException("User", request.Email);

        var user = request.ToEntity();
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        user.CreatedAt = DateTime.UtcNow;
        user.CreatedBy = currentUserService.UserId;

        var createdUser = await userRepository.CreateAsync(user);
        return createdUser.ToDto();
    }

    public async Task<UserResponseDto> UpdateAsync(Guid id, UserRequestDto request)
    {
        var user = await userRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("User", id);

        // Check email uniqueness if email is changing
        if (!user.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase))
        {
            if (await userRepository.EmailExistsAsync(request.Email))
                throw new ConflictException("User", request.Email);
        }

        request.UpdateEntity(user);
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedBy = currentUserService.UserId;

        // Only update password if provided
        if (!string.IsNullOrWhiteSpace(request.Password))
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var updatedUser = await userRepository.UpdateAsync(user);
        return updatedUser.ToDto();
    }

    public async Task DeleteAsync(Guid id)
    {
        if (!await userRepository.ExistsAsync(id))
            throw new NotFoundException("User", id);

        await userRepository.DeleteAsync(id);
    }

    public async Task ChangePasswordAsync(Guid id, ChangePasswordRequestDto request)
    {
        var user = await userRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("User", id);

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            throw new ValidationException("Current password is incorrect.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedBy = currentUserService.UserId;

        await userRepository.UpdateAsync(user);
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        var user = await userRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("User", id);

        user.DeletedAt = DateTime.UtcNow;
        user.DeletedBy = currentUserService.UserId;

        await userRepository.UpdateAsync(user);
    }

    public async Task SetRoleAsync(Guid userId, Guid roleId)
    {
        var user = await userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException("User", userId);

        if (!await roleRepository.ExistsAsync(roleId))
            throw new NotFoundException("Role", roleId);

        user.RoleId = roleId;
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedBy = currentUserService.UserId;

        await userRepository.UpdateAsync(user);
    }
}
