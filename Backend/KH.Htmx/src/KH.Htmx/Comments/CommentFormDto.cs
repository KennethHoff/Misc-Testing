using FluentValidation;
using KH.Htmx.Domain.Comments;
using KH.Htmx.Domain.People;
using KH.Htmx.Domain.Shared;

namespace KH.Htmx.Comments;

public readonly record struct CommentFormDto
{
    public required string? Text { get; init; }
    public required string? FirstName { get; init; }
    public required string? LastName { get; init; }

    // TODO: Use MediatR instead
    public Comment ToCommentEntity(TimeProvider timeProvider)
        => new()
        {
            Author = new Person
            {
                Name = FirstName is null || LastName is null
                    ? Name.Anonymous
                    : new Name
                    {
                        First = FirstName,
                        Last = LastName,
                    },
            },
            Text = Text ?? string.Empty,
            Timestamp = timeProvider.GetUtcNow(),
        };
}

internal sealed class CommentFormDtoValidator : AbstractValidator<CommentFormDto>
{
    public CommentFormDtoValidator()
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
