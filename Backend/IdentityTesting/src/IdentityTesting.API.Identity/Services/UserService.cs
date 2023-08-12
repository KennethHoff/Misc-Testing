using Microsoft.AspNetCore.Identity;
using OneOf;
using OneOf.Types;

namespace IdentityTesting.API.Identity.Services;

public sealed class UserService(
    KhUserManager userManager,
    KhRoleManager roleManager,
    KhSignInManager signInManager
)
{
    public async ValueTask<OneOf<Success, UserNotFound, RoleNotFound, UserAlreadyInRole, UnknownError>> AddRoleToUserAsync(
        string userName, string role)
    {
        if (await userManager.FindByNameAsync(userName) is not {} user)
        {
            return new UserNotFound();
        }

        if (!await roleManager.RoleExistsAsync(role))
        {
            return new RoleNotFound();
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            return new UserAlreadyInRole();
        }

        var addToRoleResult = await userManager.AddToRoleAsync(user, role);

        if (!addToRoleResult.Succeeded)
        {
            return new UnknownError(addToRoleResult.Errors);
        }

        await signInManager.RefreshSignInAsync(user);

        return new Success();
    }
    
    public async ValueTask<OneOf<Success, UserNotFound, RoleNotFound, UserNotInRole, UnknownError>> RemoveRoleFromUserAsync(
        string userName, string role)
    {
        if (await userManager.FindByNameAsync(userName) is not {} user)
        {
            return new UserNotFound();
        }

        if (!await roleManager.RoleExistsAsync(role))
        {
            return new RoleNotFound();
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            return new UserNotInRole();
        }

        var removeFromRoleResult = await userManager.RemoveFromRoleAsync(user, role);

        if (!removeFromRoleResult.Succeeded)
        {
            return new UnknownError(removeFromRoleResult.Errors);
        }
        
        await signInManager.RefreshSignInAsync(user);
        
        return new Success();
    }
}

public readonly record struct UnknownError(IEnumerable<IdentityError> Errors);

public readonly struct UserNotFound;

public readonly struct RoleNotFound;

public readonly struct UserAlreadyInRole;

public readonly struct UserNotInRole;
