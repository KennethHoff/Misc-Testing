using Microsoft.AspNetCore.Mvc;

namespace KHtmx.Account;

public sealed record class LoginFormDto
{
    [FromForm(Name = "username")] public string Username { get; init; } = null!;

    [FromForm(Name = "password")] public string Password { get; init; } = null!;

    [FromForm(Name = "rememberMe")] public bool RememberMe { get; init; }
}
