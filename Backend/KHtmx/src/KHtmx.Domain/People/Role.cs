using Microsoft.AspNetCore.Identity;

namespace KHtmx.Domain.People;

public sealed class Role(string name) : IdentityRole<Guid>(name);
