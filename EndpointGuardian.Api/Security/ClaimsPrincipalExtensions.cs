using System.Security.Claims;

namespace EndpointGuardian.Api.Security;

public static class ClaimsPrincipalExtensions
{
    public static string GetActorId(this ClaimsPrincipal user)
    {
        return user.FindFirst("sub")?.Value
            ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? user.Identity?.Name
            ?? "unknown";
    }

    var assignedBy = User.GetActorId();
}

