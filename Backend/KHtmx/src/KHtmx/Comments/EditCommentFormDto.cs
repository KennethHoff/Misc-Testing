using FluentValidation;
using KHtmx.Domain.Comments;

namespace KHtmx.Comments;

public readonly record struct EditCommentFormDto
{
    public required Guid Id { get; init; }
    public required string Text { get; init; }

    public static EditCommentFormDto FromCommentEntity(Comment comment)
        => new()
        {
            Text = comment.Text,
            Id = comment.Id,
        };
}

internal sealed class EditCommentFormDtoValidator : AbstractValidator<EditCommentFormDto>
{
    public EditCommentFormDtoValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage("Text is required");
    }
}
