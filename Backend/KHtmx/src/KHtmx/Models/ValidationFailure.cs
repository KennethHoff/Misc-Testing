using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Identity;

namespace KHtmx.Models;

public sealed record class ValidationFailure
{
    public required string ErrorMessage { get; init; }
    public string? PropertyName { get; init; }

    // public object? AttemptedValue { get; init; }
    // public object? CustomState { get; init; }
    // public required Severity Severity { get; init; }
}

public sealed class ValidationFailureCollection : Collection<ValidationFailure>
{
    public ValidationFailureCollection() : base(new List<ValidationFailure>())
    {
    }

    public ValidationFailureCollection(IList<ValidationFailure> list) : base(list)
    {
    }

    public bool PropertyHasErrors(string propertyName) => this.Any(x => x.PropertyName == propertyName);
}

public static class ValidationFailureExtensions
{
    public static ValidationFailureCollection ToValidationFailures(this IEnumerable<IdentityError> errors)
    {
        return new ValidationFailureCollection(errors.Select(x => new ValidationFailure
        {
            PropertyName = x.Code,
            ErrorMessage = x.Description,
        }).ToArray());
    }

    public static ValidationFailureCollection ToValidationFailures(this IEnumerable<FluentValidation.Results.ValidationFailure> errors)
    {
        return new ValidationFailureCollection(errors.Select(x => new ValidationFailure
        {
            PropertyName = x.PropertyName,
            ErrorMessage = x.ErrorMessage,
        }).ToArray());
    }
}
