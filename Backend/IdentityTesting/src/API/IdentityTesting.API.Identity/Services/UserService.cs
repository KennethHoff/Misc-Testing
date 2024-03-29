using IdentityTesting.API.Identity.Models;
using IdentityTesting.API.Identity.Services.Models;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace IdentityTesting.API.Identity.Services;

public sealed class UserService(
    KhUserManager userManager,
    KhRoleManager roleManager,
    KhSignInManager signInManager
)
{
    public async ValueTask<List<KhApplicationUser>> GetAllUsersAsync()
    {
        return await userManager.Users.ToListAsync();
    }

    public async ValueTask<OneOf<KhApplicationUser, UserNotFound>> GetUserAsync(string userName)
    {
        if (await userManager.FindByNameAsync(userName) is not { } user)
        {
            return new UserNotFound();
        }

        return user;
    }

    public async ValueTask<OneOf<Success, UserNotFound, RoleNotFound, UserAlreadyInRole, UnknownIdentityError>> AddRoleToUserAsync(
        string userName, string roleName)
    {
        if (await userManager.FindByNameAsync(userName) is not { } user)
        {
            return new UserNotFound();
        }

        if (!await roleManager.RoleExistsAsync(roleName))
        {
            return new RoleNotFound();
        }

        if (await userManager.IsInRoleAsync(user, roleName))
        {
            return new UserAlreadyInRole();
        }

        var addToRoleResult = await userManager.AddToRoleAsync(user, roleName);

        if (!addToRoleResult.Succeeded)
        {
            return new UnknownIdentityError(addToRoleResult.Errors);
        }

        await signInManager.RefreshSignInAsync(user);

        return new Success();
    }

    public async ValueTask<OneOf<Success, UserNotFound, RoleNotFound, UserNotInRole, UnknownIdentityError>> RemoveRoleFromUserAsync(
        string userName, string roleName)
    {
        if (await userManager.FindByNameAsync(userName) is not { } user)
        {
            return new UserNotFound();
        }

        if (!await roleManager.RoleExistsAsync(roleName))
        {
            return new RoleNotFound();
        }

        if (!await userManager.IsInRoleAsync(user, roleName))
        {
            return new UserNotInRole();
        }

        var removeFromRoleResult = await userManager.RemoveFromRoleAsync(user, roleName);

        if (!removeFromRoleResult.Succeeded)
        {
            return new UnknownIdentityError(removeFromRoleResult.Errors);
        }

        await signInManager.RefreshSignInAsync(user);

        return new Success();
    }
}

public readonly struct UserNotFound;

public readonly struct RoleNotFound;

public readonly struct UserAlreadyInRole;

public readonly struct UserNotInRole;
