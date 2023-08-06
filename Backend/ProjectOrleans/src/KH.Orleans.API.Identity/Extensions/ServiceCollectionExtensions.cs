using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KH.Orleans.API.Identity.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKhIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication();
        services.AddAuthorization(options =>
        {
            options.AddClaimPolicies();
        });
        services.AddDbContext<KhDbContext>(contextOptions =>
        {
            contextOptions.UseNpgsql(configuration.GetConnectionString("Npgsql"), npgsqlOptions =>
            {
                npgsqlOptions.MigrationsHistoryTable("__IdentityMigrationsHistory");
            });
        });

        services.AddSingleton<IStartupFilter, MigrationStartupFilter>();

        services.AddIdentityApiEndpoints<KhApplicationUser>()
            .AddEntityFrameworkStores<KhDbContext>()
            .AddDefaultTokenProviders();
        
        return services;
    }
}

public static class AppBuilderExtensions
{
    public static IApplicationBuilder MapKhIdentity(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        
        var identityGroup = app.MapGroup("/identity");

        identityGroup.MapIdentityApi<KhApplicationUser>();

        identityGroup.MapPatch("/claims",
            async ValueTask<Results<Ok, UnauthorizedHttpResult, NotFound<ClaimError>, ProblemHttpResult>> (HttpContext context,
                UserManager<KhApplicationUser> userManager, Claims claim) =>
            {
                if (context.User.Identity is null or { IsAuthenticated: false } or { Name: null })
                {
                    return TypedResults.Unauthorized();
                }

                if (await userManager.FindByNameAsync(context.User.Identity.Name) is not {} user)
                {
                    return TypedResults.NotFound(ClaimError.UserNotFound);
                }

                var claimName = claim.GetName();
                if (context.User.HasClaim(claimName, "true"))
                {
                    return TypedResults.NotFound(ClaimError.ClaimAlreadyExists);
                }

                var result = await userManager.AddClaimAsync(user, new Claim(claimName, "true"));

                return result.Succeeded switch
                {
                    true => TypedResults.Ok(),
                    _ => TypedResults.Problem(new ProblemDetails
                    {
                        Title = "Failed to add claim",
                        Detail = string.Join(", ", result.Errors.Select(e => e.Description)),
                        Status = StatusCodes.Status400BadRequest,
                    })
                };
            })
            .RequireAuthorization(opt =>
            {
                opt.RequireClaim(Claims.Admin.GetName());
            });
        
        return app;
    }
}

file enum ClaimError
{
    UserNotFound,
    ClaimNotFound,
    ClaimAlreadyExists,
}

file sealed class MigrationStartupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        => app =>
        {
            using var scope = app.ApplicationServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<KhDbContext>();
            dbContext.Database.Migrate();
            next(app);
        };
}
