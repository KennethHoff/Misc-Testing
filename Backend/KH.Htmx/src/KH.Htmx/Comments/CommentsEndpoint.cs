using FluentValidation;
using KH.Htmx.Components.Components;
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
            [FromForm] CommentFormDto dto) =>
        {
            if (await validator.ValidateAsync(dto) is { IsValid: false } validationResult)
            {
                return new RazorComponentResult<CommentForm>(new
                {
                    Comment = dto,
                    Errors = validationResult.Errors.Select(x => x.ErrorMessage).ToArray(),
                });
            }

            commentService.AddComment(dto.ToComment(TimeProvider.System));

            return new RazorComponentResult<CommentForm>();
        });

        route.MapGet("/comments", () => new RazorComponentResult<CommentTable>());
    }
}
