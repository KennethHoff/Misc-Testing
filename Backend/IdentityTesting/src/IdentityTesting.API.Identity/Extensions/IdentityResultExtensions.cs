using IdentityTesting.API.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityTesting.API.Identity.Extensions;

public static class IdentityResultExtensions
{
    public static ApiError? GetError(this IdentityResult result, IdentityError error)
    {
        if (result.HasError(error))
        {
            return new ApiError
            {
                Code = error.Code,
                Detail = error.Description
            };
        }

        return null;
    }

    public static bool HasError(this IdentityResult result, IdentityError error)
    {
        return result.Errors.Any(x => error.Code == x.Code);
    }
}
