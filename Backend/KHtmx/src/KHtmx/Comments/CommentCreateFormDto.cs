using FluentValidation;

namespace KHtmx.Comments;

public readonly record struct CommentCreateFormDto
{
    public required string Text { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
}

internal sealed class CreateCommentFormDtoValidator : AbstractValidator<CommentCreateFormDto>
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
