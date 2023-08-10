using Microsoft.AspNetCore.Authorization;

namespace IdentityTesting.API.Identity;

public static class Policies
{
    public static void RequireAdmin(AuthorizationPolicyBuilder obj) => obj.RequireRole("admin");
}
