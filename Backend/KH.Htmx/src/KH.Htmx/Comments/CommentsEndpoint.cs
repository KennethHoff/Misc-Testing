using FluentValidation;
using KH.Htmx.Components.Components;
using Lib.AspNetCore.ServerSentEvents;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace KH.Htmx.Comments;

public static class CommentsEndpointExtensions
{
    public static IServiceCollection AddComments(this IServiceCollection services)
    {
        services.AddSingleton<ICommentService, CommentService>();
        return services;
    }

    public static void MapComments(this IEndpointRouteBuilder route)
    {
        route.MapPost("/comments", async Task<RazorComponentResult<CommentForm>> (
            IValidator<CommentFormDto> validator,
            ICommentService commentService,
            [FromForm] string? firstName,
            [FromForm] string? lastName,
            [FromForm] string? text) =>
        {
            var comment = new CommentFormDto
            {
                Text = text,
                FirstName = firstName,
                LastName = lastName,
            };

            if (await validator.ValidateAsync(comment) is { IsValid: false } validationResult)
            {
                return new RazorComponentResult<CommentForm>(new
                {
                    Comment = comment,
                    Errors = validationResult.Errors.Select(x => x.ErrorMessage).ToArray(),
                });
            }

            commentService.AddComment(comment.ToComment(TimeProvider.System));

            return new RazorComponentResult<CommentForm>();
        });

        route.MapGet("/comments", () => new RazorComponentResult<CommentList>());
    }
}
