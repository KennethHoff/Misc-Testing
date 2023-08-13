using IdentityTesting.API.Identity.Models;
using IdentityTesting.API.Identity.Services;
using IdentityTesting.API.Identity.Services.Models;
using IdentityTesting.API.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace IdentityTesting.API.Identity.Endpoints;

public static class UserEndpoints
{
    private const string GetAllUsersEndpointName = "GetAllUsers";
    private const string GetUserEndpointName = "GetUser";
    private const string AddRoleToUserEndpointName = "AddRoleToUser";
    private const string RemoveRoleFromUserEndpointName = "RemoveRoleFromUser";

    private static readonly NotFound<ApiError> UserNotFoundResult = TypedResults.NotFound(new ApiError
    {
        Code = "UserNotFound",
        Detail = "User not found"
    });

    private static readonly NotFound<ApiError> RoleNotFoundResult = TypedResults.NotFound(new ApiError
    {
        Code = "RoleNotFound",
        Detail = "Role not found",
    });

    private static readonly BadRequest<ApiError> UserAlreadyInRoleResult = TypedResults.BadRequest(new ApiError
    {
        Code = "UserAlreadyInRole",
        Detail = "User is already in role"
    });

    private static readonly BadRequest<ApiError> UserNotInRoleResult = TypedResults.BadRequest(new ApiError
    {
        Code = "UserNotInRole",
        Detail = "User is not in role"
    });

    private static ProblemHttpResult RemoveRoleFromUserErrorResult(IEnumerable<IdentityError> errors) => TypedResults.Problem(new ProblemDetails
    {
        Title = "Error removing role from user",
        Extensions = { ["errors"] = errors }
    });

    private static ProblemHttpResult AddRoleToUserErrorResult(IEnumerable<IdentityError> errors) => TypedResults.Problem(new ProblemDetails
    {
        Title = "Error adding role to user",
        Extensions = { ["errors"] = errors }
    });

    public static RouteGroupBuilder MapUserEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetAllUsersEndPointHandler)
            .WithName(GetAllUsersEndpointName);
        
        group.MapGet("/{userName}", GetUserEndPointHandler)
            .WithName(GetUserEndpointName);
        
        group.MapPatch("/add-role", AddRoleToUserEndPointHandler)
            .WithName(AddRoleToUserEndpointName);

        group.MapPatch("/remove-role", RemoveRoleFromUserEndPointHandler)
            .WithName(RemoveRoleFromUserEndpointName);

        return group;
    }
    
    private static async ValueTask<List<KhApplicationUser>>
        GetAllUsersEndPointHandler(UserService userService, CancellationToken ct)
    {
        return await userService.GetAllUsersAsync();
    }
    
    private static async ValueTask<Results<NotFound<ApiError>, Ok<KhApplicationUser>>>
        GetUserEndPointHandler(UserService userService, string userName, CancellationToken ct)
    {
        var result = await userService.GetUserAsync(userName);

        return result.Value switch
        {
            UserNotFound => UserNotFoundResult,
            KhApplicationUser x => TypedResults.Ok(x),
        };
    }

    private static async ValueTask<Results<NotFound<ApiError>, BadRequest<ApiError>, ProblemHttpResult, Ok>>
        AddRoleToUserEndPointHandler(UserService userService, string userName, string role)
    {
        var result = await userService.AddRoleToUserAsync(userName, role);

        return result.Value switch
        {
            OneOf.Types.Success => TypedResults.Ok(),
            RoleNotFound => RoleNotFoundResult,
            UserNotFound => UserNotFoundResult,
            UserAlreadyInRole => UserAlreadyInRoleResult,
            UnknownIdentityError x => AddRoleToUserErrorResult(x.Errors)
        };
    }

    private static async ValueTask<Results<NotFound<ApiError>, BadRequest<ApiError>, Ok, ProblemHttpResult>>
        RemoveRoleFromUserEndPointHandler(UserService userService, string userName, string role)
    {
        var result = await userService.RemoveRoleFromUserAsync(userName, role);

        return result.Value switch
        {
            OneOf.Types.Success => TypedResults.Ok(),
            RoleNotFound => RoleNotFoundResult,
            UserNotFound => UserNotFoundResult,
            UserNotInRole => UserNotInRoleResult,
            UnknownIdentityError x => RemoveRoleFromUserErrorResult(x.Errors)
        };
    }
}
