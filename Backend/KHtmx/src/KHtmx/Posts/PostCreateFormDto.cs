using FluentValidation;

namespace KHtmx.Posts;

public readonly record struct PostCreateFormDto
{
    public required string Title { get; init; }
    public required string Content { get; init; }
}

public sealed class PostCreateFormDtoValidator : AbstractValidator<PostCreateFormDto>
{
    public PostCreateFormDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required");

        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Content is required");
    }
}
