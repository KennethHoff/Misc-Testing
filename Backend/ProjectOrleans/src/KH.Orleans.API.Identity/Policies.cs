using Microsoft.AspNetCore.Authorization;

namespace KH.Orleans.API.Identity;

public static class Policies
{
    public static void RequireAdmin(AuthorizationPolicyBuilder obj) => obj.RequireRole("admin");
}
