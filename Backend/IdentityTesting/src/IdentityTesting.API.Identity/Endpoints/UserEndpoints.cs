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
    private static NotFound<ApiDetails> NotFoundUserResult(string title) => TypedResults.NotFound(new ApiDetails
    {
        Code = title,
        Detail = "User not found"
    });

    public static RouteGroupBuilder MapUserEndpoints(this RouteGroupBuilder group)
    {
        group.MapPatch("/add-role", AddRoleToUserEndPointHandler)
            .WithName("AddRoleToUser");

        group.MapPatch("/remove-role", RemoveRoleFromUserEndPointHandler)
            .WithName("RemoveRoleFromUser");

        return group;
    }

    private static async ValueTask<Results<NotFound<ApiDetails>, BadRequest<ApiDetails>, ProblemHttpResult, Ok>>
        AddRoleToUserEndPointHandler(
            KhUserManager userManager, KhSignInManager signInManager, string userName, string role,
            CancellationToken ct)
    {
        if (await userManager.FindByNameAsync(userName) is not { } user)
        {
            return NotFoundUserResult("Failed to add role");
        }

        if (await userManager.AddToRoleAsync(user, role) is { Succeeded: false } result)
        {
            if (result.GetError(userManager.ErrorDescriber.UserAlreadyInRole(role)) is { } error)
            {
                return TypedResults.BadRequest(error);
            }

            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Failed to add role",
                Detail = string.Join(", ", result.Errors.Select(e => e.Description)),
                Status = StatusCodes.Status500InternalServerError
            });
        }

        await signInManager.RefreshSignInAsync(user);

        return TypedResults.Ok();
    }

    private static async ValueTask<Results<NotFound<ApiDetails>, BadRequest<ApiDetails>, Ok, ProblemHttpResult>>
        RemoveRoleFromUserEndPointHandler(
            KhUserManager userManager, KhSignInManager signInManager, string userName, string role)
    {
        if (await userManager.FindByNameAsync(userName) is not { } user)
        {
            return NotFoundUserResult("Failed to remove role");
        }

        var result = await userManager.RemoveFromRoleAsync(user, role);

        if (!result.Succeeded)
        {
            if (result.GetError(userManager.ErrorDescriber.UserNotInRole(role)) is { } error)
            {
                return TypedResults.BadRequest(error);
            }

            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Failed to remove role",
                Detail = string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)),
            });
        }

        await signInManager.RefreshSignInAsync(user);

        return TypedResults.Ok();
    }
}

file static class IdentityResultExtensions
{
    public static ApiDetails? GetError(this IdentityResult result, IdentityError error)
    {
        if (result.Errors.Any(x => error.Code == x.Code))
        {
            return new ApiDetails
            {
                Code = error.Code,
                Detail = error.Description
            };
        }

        return null;
    }
}
