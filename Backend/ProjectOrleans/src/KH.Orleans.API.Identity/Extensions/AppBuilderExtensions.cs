using KH.Orleans.API.Identity.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace KH.Orleans.API.Identity.Extensions;

public static class AppBuilderExtensions
{
    public static IApplicationBuilder MapKhIdentity(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        var identityGroup = app.MapGroup("/identity")
            .RequireAuthorization(Policies.RequireAdmin)
            .WithTags("Identity");

        // We need to allow login for anonymous users... duh
        identityGroup.MapIdentityApi<KhApplicationUser>()
            .AllowAnonymous();

        identityGroup.MapGroup("/roles")
            .MapRoleEndpoints();

        identityGroup.MapGroup("/users")
            .MapUserEndpoints();

        return app;
    }
}
