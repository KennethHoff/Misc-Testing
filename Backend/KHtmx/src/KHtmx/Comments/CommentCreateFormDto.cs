using FluentValidation;

namespace KHtmx.Comments;

public readonly record struct CommentCreateFormDto
{
    public required string Text { get; init; }
}

internal sealed class CreateCommentFormDtoValidator : AbstractValidator<CommentCreateFormDto>
{
    public CreateCommentFormDtoValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage("Text is required");
    }
}
