using Microsoft.AspNetCore.Identity;

namespace KHtmx.Domain.People;

public sealed class KhtmxUser : IdentityUser<Guid>
{
    [ProtectedPersonalData] public string FirstName { get; init; } = string.Empty;

    [ProtectedPersonalData] public string LastName { get; init; } = string.Empty;

    public static KhtmxUser Create(string userName, string email, string firstName, string lastName)
        => new()
        {
            FirstName = firstName,
            LastName = lastName,
            UserName = userName,
            Email = email
        };
}
