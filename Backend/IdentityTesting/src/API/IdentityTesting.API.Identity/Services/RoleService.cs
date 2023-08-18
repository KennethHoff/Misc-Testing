using IdentityTesting.API.Identity.Models;
using IdentityTesting.API.Identity.Services.Models;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace IdentityTesting.API.Identity.Services;

public sealed class RoleService(
    KhRoleManager roleManager
    )
{
    public async ValueTask<OneOf<Success, RoleAlreadyExists, UnknownIdentityError>> CreateRoleAsync(string roleName)
    {
        if (await roleManager.RoleExistsAsync(roleName))
        {
            return new RoleAlreadyExists();
        }

        var result = await roleManager.CreateAsync(new KhApplicationRole
        {
            Name = roleName
        });

        if (!result.Succeeded)
        {
            return new UnknownIdentityError(result.Errors);
        }

        return new Success();
    }

    public async ValueTask<OneOf<Success, RoleNotFound, UnknownIdentityError>> DeleteRoleAsync(string roleName)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            return new RoleNotFound();
        }

        var result = await roleManager.DeleteAsync(new KhApplicationRole
        {
            Name = roleName
        });

        if (!result.Succeeded)
        {
            return new UnknownIdentityError(result.Errors);
        }

        return new Success();
    }

    public Task<List<KhApplicationRole>> GetRolesAsync(CancellationToken ct)
    {
        return roleManager.Roles.ToListAsync(ct);
    }

    public async Task<OneOf<NotFound, KhApplicationRole>> GetRoleAsync(string roleName)
    {
        if (await roleManager.FindByNameAsync(roleName) is not { } role)
        {
            return new NotFound();
        }

        return role;
    }
}

public readonly struct RoleAlreadyExists;
