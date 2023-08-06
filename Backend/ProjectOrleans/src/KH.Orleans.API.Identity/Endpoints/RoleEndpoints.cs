using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace KH.Orleans.API.Identity.Endpoints;

public static class RoleEndpoints
{
    public static RouteGroupBuilder MapRoleEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetRolesEndPointHandler)
            .WithName("GetRoles");

        group.MapGet("/{role}", GetRoleEndPointHandler)
            .WithName("GetRole");

        group.MapPost("/", CreateRoleEndPointHandler)
            .WithName("CreateRole");

        return group;
    }

    private static async ValueTask<Ok<List<KhApplicationRole>>> GetRolesEndPointHandler(KhRoleManager roleManager,
        CancellationToken ct)
    {
        var roles = await roleManager.Roles.ToListAsync(cancellationToken: ct);
        return TypedResults.Ok(roles);
    }

    private static async ValueTask<Ok<KhApplicationRole>> GetRoleEndPointHandler(KhRoleManager roleManager, string role,
        CancellationToken ct)
    {
        var roles = await roleManager.Roles.FirstOrDefaultAsync(r => r.Name == role, cancellationToken: ct);
        return TypedResults.Ok(roles);
    }

    private static async ValueTask<Results<CreatedAtRoute, ProblemHttpResult>> CreateRoleEndPointHandler(
        KhRoleManager roleManager, string role)
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
    }
}
