using KH.Orleans.API.Models;
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
        services.AddAuthorization();
        services.AddDbContext<KhDbContext>(contextOptions =>
        {
            contextOptions.UseNpgsql(configuration.GetConnectionString("Npgsql"), npgsqlOptions =>
            {
                npgsqlOptions.MigrationsHistoryTable("__IdentityMigrationsHistory");
            });
        });

        services.AddSingleton<IStartupFilter, MigrationStartupFilter>();
        // services.AddScoped<IUserClaimsPrincipalFactory<KhApplicationUser>, KhUserClaimsPrincipalFactory>();

        services.AddIdentityApiEndpoints<KhApplicationUser>()
            .AddDefaultTokenProviders()
            .AddRoles<KhApplicationRole>()
            .AddUserManager<KhUserManager>()
            .AddRoleManager<KhRoleManager>()
            .AddEntityFrameworkStores<KhDbContext>()
            .AddSignInManager<KhSignInManager>();

        return services;
    }
}

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

    private static RouteGroupBuilder MapRoleEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/{userName}",
                async ValueTask<Results<Ok<IList<string>>, NotFound<ApiDetails>>> (
                    KhUserManager userManager, string userName) =>
                {
                    if (await userManager.FindByNameAsync(userName) is not { } user)
                    {
                        return TypedResults.NotFound(new ApiDetails
                        {
                            Title = "Failed to find user",
                            Detail = "User not found"
                        });
                    }

                    return await userManager.GetRolesAsync(user) switch
                    {
                        null => TypedResults.NotFound(new ApiDetails
                        {
                            Title = "Failed to find user",
                            Detail = "User not found"
                        }),
                        { } roles => TypedResults.Ok(roles),
                    };
                })
            .WithName("GetUser");


        group.MapGet("/",
                async (KhRoleManager roleManager) =>
                {
                    var roles = await roleManager.Roles.ToListAsync();
                    return TypedResults.Ok(roles);
                })
            .WithName("GetRoles");

        group.MapGet("/{role}",
                async (KhRoleManager roleManager, string role) =>
                {
                    var roles = await roleManager.Roles.Where(r => r.Name == role).ToListAsync();
                    return TypedResults.Ok(roles);
                })
            .WithName("GetRole");

        group.MapPost("/",
                async ValueTask<Results<CreatedAtRoute, ProblemHttpResult>> (KhRoleManager roleManager, string role) =>
                {
                    var result = await roleManager.CreateAsync(new KhApplicationRole
                    {
                        Name = role
                    });
                    return result.Succeeded switch
                    {
                        true => TypedResults.CreatedAtRoute("GetRole", new
                        {
                            role
                        }),
                        _ => TypedResults.Problem(new ProblemDetails
                        {
                            Title = "Failed to create role",
                            Detail = string.Join(", ", result.Errors.Select(e => e.Description)),
                            Status = StatusCodes.Status500InternalServerError
                        })
                    };
                })
            .WithName("CreateRole");

        return group;
    }

    private static RouteGroupBuilder MapUserEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/{userName}",
                async ValueTask<Results<Ok<KhApplicationUser>, NotFound<ApiDetails>>> (
                    KhUserManager userManager, string userName) =>
                {
                    var user = await userManager.FindByNameAsync(userName);
                    return user is null
                        ? TypedResults.NotFound(new ApiDetails
                        {
                            Title = "Failed to find user",
                            Detail = "User not found"
                        })
                        : TypedResults.Ok(user);
                })
            .WithName("GetUser");

        group.MapPatch("/add-role",
                async ValueTask<Results<NotFound<ApiDetails>, BadRequest<ApiDetails>, ProblemHttpResult, Ok>> (
                    KhUserManager userManager, KhSignInManager signInManager, string userName, string role) =>
                {
                    if (await userManager.FindByNameAsync(userName) is not { } user)
                    {
                        return TypedResults.NotFound(new ApiDetails
                        {
                            Title = "Failed to add role",
                            Detail = "User not found"
                        });
                    }

                    if (await userManager.IsInRoleAsync(user, role))
                    {
                        return TypedResults.BadRequest(new ApiDetails
                        {
                            Title = "Failed to add role",
                            Detail = "User already in role"
                        });
                    }

                    var result = await userManager.AddToRoleAsync(user, role);

                    await signInManager.RefreshSignInAsync(user);

                    return result.Succeeded switch
                    {
                        true => TypedResults.Ok(),
                        _ => TypedResults.Problem(new ProblemDetails
                        {
                            Title = "Failed to add role",
                            Detail = string.Join(", ", result.Errors.Select(e => e.Description)),
                            Status = StatusCodes.Status500InternalServerError
                        })
                    };
                })
            .WithName("AddRoleToUser");

        group.MapPatch("/remove-role",
                async ValueTask<Results<NotFound<ApiDetails>, BadRequest<ApiDetails>, ProblemHttpResult, Ok>> (
                    KhUserManager userManager, KhSignInManager signInManager, string userName, string role) =>
                {
                    if (await userManager.FindByNameAsync(userName) is not { } user)
                    {
                        return TypedResults.NotFound(new ApiDetails
                        {
                            Title = "Failed to remove role",
                            Detail = "User not found",
                        });
                    }

                    if (!await userManager.IsInRoleAsync(user, role))
                    {
                        return TypedResults.BadRequest(new ApiDetails
                        {
                            Title = "Failed to remove role",
                            Detail = "User not in role",
                        });
                    }

                    var result = await userManager.RemoveFromRoleAsync(user, role);

                    await signInManager.RefreshSignInAsync(user);

                    return result.Succeeded switch
                    {
                        true => TypedResults.Ok(),
                        _ => TypedResults.Problem(new ProblemDetails
                        {
                            Title = "Failed to remove role",
                            Detail = string.Join(", ", result.Errors.Select(e => e.Description)),
                            Status = StatusCodes.Status500InternalServerError,
                        })
                    };
                })
            .WithName("RemoveRoleFromUser");

        return group;
    }
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
