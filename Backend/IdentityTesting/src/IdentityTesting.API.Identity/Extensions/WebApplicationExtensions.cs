using IdentityTesting.API.Identity.Endpoints;
using IdentityTesting.API.Identity.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityTesting.API.Identity.Extensions;

public static class WebApplicationExtensions
{
    public static IApplicationBuilder UseKhIdentity(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.ApplyMigrations();
        }

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

    private static void ApplyMigrations(this IHost app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<KhDbContext>();
        dbContext.Database.Migrate();
    }
}
