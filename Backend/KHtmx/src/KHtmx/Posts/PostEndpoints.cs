using System.Security.Claims;
using FluentValidation;
using KHtmx.Comments;
using KHtmx.Components.Posts;
using KHtmx.Constants;
using KHtmx.Domain.People;
using KHtmx.Domain.Posts;
using KHtmx.Persistence;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KHtmx.Posts;

public static class PostEndpoints
{
    public static void MapPosts(this IEndpointRouteBuilder route)
    {
        var htmxGroup = route.MapGroup(EndpointConstants.HtmxPrefix);

        htmxGroup.MapGet("GetCommentTable", GetPostTable.Handler)
            .WithName(GetPostTable.EndpointName);

        htmxGroup.MapPost("Post", CreatePost.Handler)
            .WithName(CreatePost.EndpointName);
    }

    public sealed class GetPostTable
    {
        public const string EndpointName = "GetCommentsTable";

        public static RazorComponentResult<PostTableComponent> Handler
        (
            [FromQuery(Name = "authorId")] Guid? authorId
        )
        {
            // if (authorId is { } authorGuid)
            // {
            //     return new RazorComponentResult<PostTableComponent>(new
            //     {
            //         Filter = new AuthorCommentTableFilter(authorGuid),
            //     });
            // }

            return new RazorComponentResult<PostTableComponent>();
        }
    }

    public sealed class CreatePost
    {
        public const string EndpointName = "CreatePost";
        private static readonly string[] UserNotFoundError = ["User not found"];

        public static async ValueTask<RazorComponentResult<PostCreateFormComponent>> Handler
        (
            IValidator<CommentCreateFormDto> validator,
            IDbContextFactory<KhDbContext> dbContextFactory,
            [FromForm] CommentCreateFormDto dto,
            ClaimsPrincipal claimsPrincipal,
            UserManager<KhtmxUser> userManager,
            CancellationToken ct
        )
        {
            if (await validator.ValidateAsync(dto, ct) is { IsValid: false } validationResult)
            {
                return new RazorComponentResult<PostCreateFormComponent>(new
                {
                    Comment = dto,
                    Errors = validationResult.Errors.Select(x => x.ErrorMessage).ToArray(),
                });
            }

            await using var dbContext = await dbContextFactory.CreateDbContextAsync(ct);

            if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
            {
                return new RazorComponentResult<PostCreateFormComponent>(new
                {
                    Comment = dto,
                    Errors = UserNotFoundError,
                });
            }

            DateTimeOffset timestamp = TimeProvider.System.GetUtcNow();
            var entity = Post.Create(dto.Text, timestamp, user.Id);

            dbContext.Add(entity);
            await dbContext.SaveChangesAsync(ct);

            return new RazorComponentResult<PostCreateFormComponent>(new
            {
                Comment = new PostCreateFormDto
                {
                    Content = string.Empty,
                    Title = string.Empty,
                },
            });
        }
    }

}
