using FluentValidation;

namespace IdentityTesting.Web.Models;

internal sealed class InputModel
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

internal sealed class InputModelValidator : AbstractValidator<InputModel>
{
    public InputModelValidator()
    {
        RuleFor(x => x.Email).NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email is not valid")
            .Must(x => x.Contains("@gmail.com"))
            .WithMessage("Email must be gmail");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters");
    }
}
