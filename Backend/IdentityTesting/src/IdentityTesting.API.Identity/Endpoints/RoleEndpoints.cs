using IdentityTesting.API.Identity.Models;
using IdentityTesting.API.Identity.Services;
using IdentityTesting.API.Identity.Services.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using OneOf.Types;
using NotFound = Microsoft.AspNetCore.Http.HttpResults.NotFound;

namespace IdentityTesting.API.Identity.Endpoints;

public static class RoleEndpoints
{
    private const string GetRolesEndpointName = "GetRoles";
    private const string CreateRoleEndpointName = "CreateRole";
    private const string GetRoleEndpointName = "GetRole";
    private const string DeleteRoleEndpointName = "DeleteRole";

    private static ProblemHttpResult DeleteRoleErrorResult(IEnumerable<IdentityError> errors)
        => TypedResults.Problem(new ProblemDetails
        {
            Title = "Failed to delete role",
            Extensions = { ["errors"] = errors }
        });

    private static ProblemHttpResult CreateRoleErrorResult(IEnumerable<IdentityError> errors)
        => TypedResults.Problem(new ProblemDetails
        {
            Title = "Failed to create role",
            Extensions = { ["errors"] = errors }
        });

    public static RouteGroupBuilder MapRoleEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetRolesEndPointHandler)
            .WithName(GetRolesEndpointName);

        group.MapPost("/", CreateRoleEndPointHandler)
            .WithName(CreateRoleEndpointName);

        group.MapGet("/{role}", GetRoleEndPointHandler)
            .WithName(GetRoleEndpointName);

        group.MapDelete("/{role}", DeleteRoleEndPointHandler)
            .WithName(DeleteRoleEndpointName);

        return group;
    }

    private static async ValueTask<Ok<List<KhApplicationRole>>> GetRolesEndPointHandler(
        RoleService roleService, CancellationToken ct)
    {
        var result = await roleService.GetRolesAsync(ct);

        return TypedResults.Ok(result);
    }

    private static async ValueTask<Results<NotFound, Ok<KhApplicationRole>>> GetRoleEndPointHandler(
        RoleService roleService, string role)
    {
        var result = await roleService.GetRoleAsync(role);

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
            Success => TypedResults.CreatedAtRoute(GetRoleEndpointName, new RouteValueDictionary
            {
                ["role"] = role
            }),
            RoleAlreadyExists => TypedResults.Conflict(),
            UnknownIdentityError errorResult => CreateRoleErrorResult(errorResult.Errors)
        };
    }

    private static async ValueTask<Results<NotFound, Ok, ProblemHttpResult>> DeleteRoleEndPointHandler(
        RoleService roleService, string role)
    {
        var result = await roleService.DeleteRoleAsync(role);

        return result.Value switch
        {
            Success => TypedResults.Ok(),
            RoleNotFound => TypedResults.NotFound(),
            UnknownIdentityError x => DeleteRoleErrorResult(x.Errors)
        };
    }
}
