using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using OneOf;
using OneOf.Types;

namespace IdentityTesting.API.Identity.Services;

public sealed class RoleService(KhRoleManager roleManager, ILogger<RoleService> logger)
{
    public async ValueTask<OneOf<Success, RoleAlreadyExistsResult, RoleCreationErrorResult>> CreateRoleAsync(string role)
    {
        var result = await roleManager.CreateAsync(new KhApplicationRole
        {
            Name = role
        });

        if (result.Errors.Any(e => e.Code == roleManager.ErrorDescriber.DuplicateRoleName(role).Code))
        {
            return new RoleAlreadyExistsResult();
        }

        if (!result.Succeeded)
        {
            logger.LogError("Failed to create role {Role}: {@Errors}", role, result.Errors);
            return new RoleCreationErrorResult(result.Errors);
        }
        
        return new Success();
    }
}

public readonly struct RoleAlreadyExistsResult;
public readonly record struct RoleCreationErrorResult(IEnumerable<IdentityError> Errors);
