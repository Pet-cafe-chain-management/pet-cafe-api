using PetCafe.Application.Services.Commons;
using System.Security.Claims;

namespace PetCafe.WebApi.Services;

public class ClaimService : IClaimsService
{
    public ClaimService(IHttpContextAccessor httpContextAccessor)
    {
        var httpContext = httpContextAccessor.HttpContext;
        var user = httpContext?.User;

        var id = user?.FindFirstValue(ClaimTypes.NameIdentifier);
        var role = user?.FindFirstValue(ClaimTypes.Role);

        GetCurrentUser = string.IsNullOrEmpty(id) ? Guid.Empty : Guid.Parse(id);
        GetCurrentUserRole = role ?? string.Empty;
    }
    public Guid GetCurrentUser { get; }
    public string GetCurrentUserRole { get; }
}
