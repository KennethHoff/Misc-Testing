using Microsoft.AspNetCore.Identity;

namespace KHtmx.Domain.People;

public sealed class User : IdentityUser<Guid>
{
    [ProtectedPersonalData] public string FirstName { get; init; } = string.Empty;

    [ProtectedPersonalData] public string LastName { get; init; } = string.Empty;

    public User()
    {
    }

    public static User Create(string firstName, string lastName, string userName, string email)
        => new()
        {
            FirstName = firstName,
            LastName = lastName,
            UserName = userName,
            Email = email
        };
}
