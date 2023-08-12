using Microsoft.AspNetCore.Identity;

namespace IdentityTesting.API.Identity.Services.Models;

public readonly record struct UnknownIdentityError(IEnumerable<IdentityError> Errors);
