using IdentityTesting.API.Identity.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace IdentityTesting.API.Identity.Services;

public sealed class RoleService(
    KhRoleManager roleManager 
    )
{
    public async ValueTask<OneOf<Success, RoleAlreadyExistsResult, RoleCreationErrorResult>>
        CreateRoleAsync(string role)
    {
        var result = await roleManager.CreateAsync(new KhApplicationRole
        {
            Name = role
        });

        if (!result.Succeeded)
        {
            if (result.HasError(roleManager.ErrorDescriber.DuplicateRoleName(role)))
            {
                return new RoleAlreadyExistsResult();
            }
            
            return new RoleCreationErrorResult(result.Errors);
        }

        return new Success();
    }

    public async ValueTask<OneOf<Success, NotFound>> DeleteRoleAsync(string role)
    {
        var result = await roleManager.DeleteAsync(new KhApplicationRole
        {
            Name = role
        });

        if (!result.Succeeded)
        {
            return new NotFound();
        }

        return new Success();
    }

    public async Task<OneOf<List<KhApplicationRole>>> GetRolesAsync(CancellationToken ct)
    {
        return await roleManager.Roles.ToListAsync(ct);
    }
    
    public async Task<OneOf<NotFound, KhApplicationRole>> GetRoleAsync(string role, CancellationToken ct)
    {
        var roles = await roleManager.FindByNameAsync(role);
        
        if (roles is null)
        {
            return new NotFound();
        }
        
        return roles;
    }
}

public readonly struct RoleAlreadyExistsResult;

public readonly record struct RoleCreationErrorResult(IEnumerable<IdentityError> Errors);
