using FluentValidation;
using KHtmx.Domain.Comments;

namespace KHtmx.Components.Comments.Data;

public readonly record struct CommentEditFormDto
{
    public required Guid Id { get; init; }
    public required string Text { get; init; }

    public static CommentEditFormDto FromCommentEntity(Comment comment)
        => new()
        {
            Text = comment.Text,
            Id = comment.Id,
        };
}

internal sealed class CommentEditFormDtoValidator : AbstractValidator<CommentEditFormDto>
{
    public CommentEditFormDtoValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage("Text is required");
    }
}
