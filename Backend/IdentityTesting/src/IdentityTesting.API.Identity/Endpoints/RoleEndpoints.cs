using IdentityTesting.API.Identity.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using OneOf.Types;
using NotFound = Microsoft.AspNetCore.Http.HttpResults.NotFound;

namespace IdentityTesting.API.Identity.Endpoints;

public static class RoleEndpoints
{
    public static RouteGroupBuilder MapRoleEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetRolesEndPointHandler)
            .WithName("GetRoles");

        group.MapPost("/", CreateRoleEndPointHandler)
            .WithName("CreateRole");

        group.MapGet("/{role}", GetRoleEndPointHandler)
            .WithName("GetRole");

        group.MapDelete("/{role}", DeleteRoleEndPointHandler)
            .WithName("DeleteRole");

        return group;
    }

    private static async ValueTask<Ok<List<KhApplicationRole>>> GetRolesEndPointHandler(RoleService roleService,
        CancellationToken ct)
    {
        var result = await roleService.GetRolesAsync(ct);

        return result.Value switch
        {
            List<KhApplicationRole> x => TypedResults.Ok(x)
        };
    }

    private static async ValueTask<Results<NotFound, Ok<KhApplicationRole>>> GetRoleEndPointHandler(RoleService roleService, string role,
        CancellationToken ct)
    {
        var result = await roleService.GetRoleAsync(role, ct);

        return result.Value switch
        {
            OneOf.Types.NotFound x => TypedResults.NotFound(),
            KhApplicationRole x => TypedResults.Ok(x)
        };
    }

    private static async ValueTask<Results<CreatedAtRoute, Conflict, ProblemHttpResult>> CreateRoleEndPointHandler(
        RoleService roleService, string role)
    {
        var result = await roleService.CreateRoleAsync(role);

        return result.Value switch
        {
            Success => TypedResults.CreatedAtRoute("GetRole", new
            {
                role
            }),
            RoleAlreadyExistsResult => TypedResults.Conflict(),
            RoleCreationErrorResult errorResult => TypedResults.Problem(new ProblemDetails
            {
                Title = "Failed to create role",
                Detail = string.Join(", ", errorResult.Errors.Select(e => e.Description)),
                Status = StatusCodes.Status500InternalServerError
            }),
        };
    }

    private static async ValueTask<Results<NotFound, Ok>> DeleteRoleEndPointHandler(
        RoleService roleService, string role)
    {
        var result = await roleService.DeleteRoleAsync(role);

        return result.Value switch
        {
            Success => TypedResults.Ok(),
            OneOf.Types.NotFound => TypedResults.NotFound(),
        };
    }
}
