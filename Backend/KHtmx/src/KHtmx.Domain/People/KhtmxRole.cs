using Microsoft.AspNetCore.Identity;

namespace KHtmx.Domain.People;

public sealed class KhtmxRole(string name) : IdentityRole<Guid>(name);
