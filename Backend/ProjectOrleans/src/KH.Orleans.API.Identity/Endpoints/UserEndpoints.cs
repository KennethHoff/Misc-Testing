using System.ComponentModel.DataAnnotations;
using KH.Orleans.API.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace KH.Orleans.API.Identity.Endpoints;

public static class UserEndpoints
{
    /// <summary>
    /// Singleton instance of <see cref="EmailAddressAttribute"/> to avoid per-request allocations.
    /// </summary>
    private static readonly EmailAddressAttribute EmailValidator = new();
    
    public static RouteGroupBuilder MapUserEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/{userName}",
                async ValueTask<Results<ProblemHttpResult, NotFound<ApiDetails>, Ok<UserResponse>>> (
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

                    if (await userManager.GetRolesAsync(user) is not { } roles)
                    {
                        return TypedResults.Problem(new ProblemDetails
                        {
                            Title = "Failed to get user roles",
                            Detail = "User roles not found",
                        });
                    }

                    return TypedResults.Ok(new UserResponse(user, roles));
                })
            .WithName("GetUser");

        group.MapPatch("/{userName}",
                async ValueTask<Results<NotFound<ApiDetails>, BadRequest<ApiDetails>, ProblemHttpResult, Ok>> (
                    KhUserManager userManager, KhSignInManager signInManager, string userName, string email) =>
                {
                    if (await userManager.FindByNameAsync(userName) is not { } user)
                    {
                        return TypedResults.NotFound(new ApiDetails
                        {
                            Title = "Failed to change password",
                            Detail = "User not found"
                        });
                    }

                    if (!EmailValidator.IsValid(email))
                    {
                        return TypedResults.BadRequest(new ApiDetails
                        {
                            Title = "Failed to change email",
                            Detail = "Invalid email"
                        });
                    }
                    
                    if (await userManager.SetEmailAsync(user, email) is { Succeeded: false } result)
                    {
                        return TypedResults.Problem(new ProblemDetails
                        {
                            Title = "Failed to change email",
                            Detail = string.Join(", ", result.Errors.Select(e => e.Description)),
                            Status = StatusCodes.Status500InternalServerError
                        });
                    }

                    await signInManager.RefreshSignInAsync(user);
                    
                    return TypedResults.Ok();
                })
            .WithName("ChangePassword");

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

                    if (await userManager.AddToRoleAsync(user, role) is { Succeeded: false } result)
                    {
                        if (result.Errors.Any(x => userManager.ErrorDescriber.UserAlreadyInRole(role).Code == x.Code))
                        {
                            return TypedResults.BadRequest(new ApiDetails
                            {
                                Title = "Failed to add role",
                                Detail = "User already in role"
                            });
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
                })
            .WithName("AddRoleToUser");

        group.MapPatch("/remove-role",
                async ValueTask<Results<NotFound<ApiDetails>, BadRequest<ApiDetails>, Ok, ProblemHttpResult>> (
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

                    var result = await userManager.RemoveFromRoleAsync(user, role);

                    if (!result.Succeeded)
                    {
                        if (result.Errors.Any(x => userManager.ErrorDescriber.UserNotInRole(role).Code == x.Code))
                        {
                            return TypedResults.BadRequest(new ApiDetails
                            {
                                Title = "Failed to remove role",
                                Detail = "User does not have role",
                            });
                        }

                        return TypedResults.Problem(new ProblemDetails
                        {
                            Title = "Failed to remove role",
                            Detail = string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)),
                        });
                    }

                    await signInManager.RefreshSignInAsync(user);

                    return TypedResults.Ok();
                })
            .WithName("RemoveRoleFromUser");

        return group;
    }
}

file sealed record class UserResponse(KhApplicationUser User, IList<string> Roles);
