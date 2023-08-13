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

        return app;
    }

    public static IApplicationBuilder MapKhIdentity(this WebApplication app)
    {
        app.MapGroup("/identity")
            .WithTags("Identity")
            .MapIdentityApi<KhApplicationUser>();

        app.MapGroup("/roles")
            .WithTags("Roles")
            .RequireAuthorization(Policies.RequireAdmin)
            .MapRoleEndpoints();

        app.MapGroup("/users")
            .WithTags("Users")
            .RequireAuthorization(Policies.RequireAdmin)
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
