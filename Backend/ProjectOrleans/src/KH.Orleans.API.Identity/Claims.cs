using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace KH.Orleans.API.Identity;

public enum Claims
{
    [Display(Name = "Admin")]
    Admin,
}

public static class ClaimsExtensions
{
    public static string GetName(this Claims claim)
    {
        return claim switch
        {
            Claims.Admin => "Admin",
            _ => throw new ArgumentOutOfRangeException(nameof(claim), claim, null)
        };
    }

    private static AuthorizationPolicy GetPolicy(this Claims claim)
    {
        return claim switch
        {
            Claims.Admin => new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole("Admin")
                .Build(),
            _ => throw new ArgumentOutOfRangeException(nameof(claim), claim, null)
        };
    }
    
    internal static void AddClaimPolicies(this AuthorizationOptions options)
    {
        foreach (var claim in Enum.GetValues<Claims>())
        {
            options.AddPolicy(claim.GetName(), claim.GetPolicy());
        }
    }
}
