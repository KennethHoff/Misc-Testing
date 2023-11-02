using FluentValidation;
using KHtmx.Domain.Comments;
using KHtmx.Domain.People;
using KHtmx.Domain.Shared;

namespace KHtmx.Comments;

public readonly record struct CommentFormDto
{
    public required string? Text { get; init; }
    public required string? FirstName { get; init; }
    public required string? LastName { get; init; }

    // TODO: Use CQRS instead
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
    
    public static CommentFormDto FromCommentEntity(Comment comment)
        => new()
        {
            Text = comment.Text,
            FirstName = comment.Author.Name.First,
            LastName = comment.Author.Name.Last,
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
