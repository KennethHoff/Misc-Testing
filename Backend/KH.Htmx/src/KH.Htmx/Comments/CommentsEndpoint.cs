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
            IServerSentEventsService serverSentEventsService,
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

            #region SSE Stuff that should probably be moved somewhere else

            var clients = serverSentEventsService.GetClients();
            if (clients.Count is 0)
            {
                Console.WriteLine("No clients connected, not sending SSE");
                return new RazorComponentResult<CommentForm>();
            }

            Console.WriteLine($"Sending SSE to {clients.Count} clients");

            await serverSentEventsService.SendEventAsync(new ServerSentEvent
            {
                Id = "comment",
                Type = "comment",
                Data = ["loL"],
            });

            #endregion

            return new RazorComponentResult<CommentForm>();
        });

        route.MapGet("/comments", () => new RazorComponentResult<CommentList>());
    }
}
