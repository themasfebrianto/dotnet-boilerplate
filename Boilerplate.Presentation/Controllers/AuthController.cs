using Boilerplate.Application.DTOs.Auth;
using Boilerplate.Application.DTOs.User;
using Boilerplate.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Boilerplate.Presentation.Controllers;

/// <summary>
/// Authentication endpoints.
/// Controllers are thin - just call service and return.
/// Exception handling is done by middleware.
/// </summary>
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ApiController
{
    [HttpPost("login")]
    public async Task<LoginResponseDto> Login([FromBody] LoginRequestDto request)
        => await authService.LoginAsync(request);

    [HttpPost("register")]
    public async Task<LoginResponseDto> Register([FromBody] UserRequestDto request)
        => await authService.RegisterAsync(request);

    [HttpPost("refresh")]
    public async Task<LoginResponseDto> Refresh([FromBody] RefreshTokenRequestDto request)
        => await authService.RefreshTokenAsync(request.RefreshToken);

    [HttpPost("revoke")]
    public async Task Revoke([FromBody] RefreshTokenRequestDto request)
        => await authService.RevokeTokenAsync(request.RefreshToken);
}
