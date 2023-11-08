using FluentValidation;

namespace KHtmx.Components.Comments.Data;

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
            .WithMessage("Text is required")
            .MinimumLength(10)
            .WithMessage("Text must be at least 10 characters long");
    }
}
