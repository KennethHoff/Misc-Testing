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

}
