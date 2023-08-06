using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace KH.Orleans.API.Identity;

public sealed class KhUserClaimsPrincipalFactory(KhUserManager userManager, KhRoleManager roleManager,
        IOptions<IdentityOptions> options)
    : UserClaimsPrincipalFactory<KhApplicationUser, KhApplicationRole>(userManager, roleManager, options)
{
    public override async Task<ClaimsPrincipal> CreateAsync(KhApplicationUser user)
    {
        var principal = await base.CreateAsync(user);
        if (principal.Identity is not ClaimsIdentity identity)
        {
            Debug.Fail("Identity is not ClaimsIdentity");
            throw new UnreachableException("Identity is not ClaimsIdentity");
        }

        return principal;
    }
}
