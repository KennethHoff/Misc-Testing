using System.Collections.Immutable;
using KH.Htmx.Components.Components;
using Lib.AspNetCore.ServerSentEvents;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace KH.Htmx.Comments;

public static class CommentsEndpointExtensions
{
    public static IServiceCollection AddComments(this IServiceCollection services)
    {
        services.AddSingleton<CommentService>();
        return services;
    }

    public static void MapComments(this IEndpointRouteBuilder route, string endpointRoot)
    {
        route.MapPost("/comments", async Task<RazorComponentResult<CommentForm>> (
            IServerSentEventsService serverSentEventsService,
            CommentService commentService,
            [FromForm] string? comment) =>
        {
            if (string.IsNullOrWhiteSpace(comment))
            {
                return new RazorComponentResult<CommentForm>(new
                {
                    ErrorMessage = "Comment cannot be empty",
                });
            }

            commentService.AddComment(comment);

            #region SSE Stuff that should probably be moved somewhere else

            var clients = serverSentEventsService.GetClients();
            if (clients.Count is 0)
            {
                return new RazorComponentResult<CommentForm>();
            }

            var comments = commentService.GetComments();
            await serverSentEventsService.SendEventAsync(new ServerSentEvent
            {
                Id = "comment",
                Type = "comment",
                Data = comments.ToImmutableList(),
            });

            #endregion

            return new RazorComponentResult<CommentForm>();
        });

        route.MapGet(endpointRoot, () => new RazorComponentResult<CommentList>());
    }
}
