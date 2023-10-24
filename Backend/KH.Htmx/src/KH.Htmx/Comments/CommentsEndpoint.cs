using System.Collections.Immutable;
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
        route.MapPost(endpointRoot, async Task<Results<Ok, BadRequest>> (
            IServerSentEventsService serverSentEventsService,
            CommentService commentService, 
            [FromForm] string comment) =>
        {
            if (string.IsNullOrWhiteSpace(comment))
            {
                return TypedResults.BadRequest();
            }

            commentService.AddComment(comment);
            
            // This should probably be moved somewhere else. I don't know where though :(
            var clients = serverSentEventsService.GetClients();
            if (clients.Count is 0)
            {
                return TypedResults.Ok();
            }

            var comments = commentService.GetComments();
            await serverSentEventsService.SendEventAsync(new ServerSentEvent
            {
                Id = "comment",
                Type = "comment",
                Data = comments.ToImmutableList(),
            });
            
            // END OF: This should probably be moved somewhere else. I don't know where though :(

            return TypedResults.Ok();
        });

        route.MapGet(endpointRoot, () => new RazorComponentResult<CommentList>());
    }
}
