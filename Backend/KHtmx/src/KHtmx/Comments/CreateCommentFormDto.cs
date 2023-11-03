using FluentValidation;
using KHtmx.Domain.Comments;
using KHtmx.Domain.People;
using KHtmx.Domain.Shared;

namespace KHtmx.Comments;

public readonly record struct CreateCommentFormDto
{
    public required string Text { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }

    // TODO: Use CQRS instead
    public Comment ToCommentEntity(TimeProvider timeProvider)
        => new()
        {
            Author = new Person
            {
                Name = new Name(FirstName, LastName),
            },
            Text = Text,
            Timestamp = timeProvider.GetUtcNow(),
        };
}

internal sealed class CreateCommentFormDtoValidator : AbstractValidator<CreateCommentFormDto>
{
    public CreateCommentFormDtoValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage("Text is required");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required");
    }
}
