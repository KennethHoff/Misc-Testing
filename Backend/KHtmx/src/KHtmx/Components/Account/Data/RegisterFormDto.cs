using Microsoft.AspNetCore.Mvc;

namespace KHtmx.Components.Account.Data;

public sealed record class RegisterFormDto
{
    [FromForm(Name = "username")] public string Username { get; init; } = null!;

    [FromForm(Name = "email")] public string Email { get; init; } = null!;

    [FromForm(Name = "password")] public string Password { get; init; } = null!;

    [FromForm(Name = "confirmPassword")] public string ConfirmPassword { get; init; } = null!;

    [FromForm(Name = "firstName")] public string FirstName { get; init; } = null!;

    [FromForm(Name = "lastName")] public string LastName { get; init; } = null!;
}
