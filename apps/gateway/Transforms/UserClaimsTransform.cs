using System.Security.Claims;
using Yarp.ReverseProxy.Transforms;

namespace gateway.Transforms;

public static class UserClaimsTransform
{
    public static ValueTask AddUserClaimsToHeaders(RequestTransformContext transformContext)
    {
        var user = transformContext.HttpContext.User;

        if (user?.Identity?.IsAuthenticated != true)
        {
            return ValueTask.CompletedTask;
        }

        var sub = user.FindFirst("sub")?.Value
                  ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var email = user.FindFirst("preferred_username")?.Value
                    ?? user.FindFirst(ClaimTypes.Email)?.Value;

        var roleClaims = user.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();

        if (!string.IsNullOrEmpty(sub))
            transformContext.ProxyRequest.Headers.Add("X-User-Id", sub);

        if (!string.IsNullOrEmpty(email))
            transformContext.ProxyRequest.Headers.Add("X-User-Email", email);

        if (roleClaims.Count > 0)
            transformContext.ProxyRequest.Headers.Add("X-User-Roles", string.Join(",", roleClaims));

        return ValueTask.CompletedTask;
    }
}