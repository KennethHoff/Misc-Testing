using FluentValidation;
using Khtmx.Persistence;
using Khtmx.Application.Comments.Commands.CreateComment;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Khtmx.Web.Comments;

public static class CommentsEndpointExtensions
{
    public static IServiceCollection AddComments(this IServiceCollection services)
    {
        return services;
    }

    public static void MapComments(this IEndpointRouteBuilder route)
    {
        route.MapPost("/comments", async Task<RazorComponentResult<CommentForm>> (
            ISender sender,
            [FromForm] CommentFormDto dto) =>
        {
            var commentId = await publisher.Publish(new CreateCommentCommand(
                dto.AuthorId,
                dto.PostId,
                dto.Text,
                TimeProvider.System.GetUtcNow()));
            
            return new RazorComponentResult<CommentForm>();
        });

        route.MapGet("/comments", () => new RazorComponentResult<CommentTable>());
    }
}
