using System.Security.Claims;
using Basirah.Application.Common.Abstractions;

namespace Basirah.Presentation.Services;

/// <summary>
/// Implementation of ICurrentUserService that reads from HttpContext.
/// </summary>
public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var id = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(id, out var userId) ? userId : null;
        }
    }

    public string? Email => User?.FindFirstValue(ClaimTypes.Email);

    public IEnumerable<string> Roles => User?
        .FindAll(ClaimTypes.Role)
        .Select(c => c.Value) ?? [];

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
}
