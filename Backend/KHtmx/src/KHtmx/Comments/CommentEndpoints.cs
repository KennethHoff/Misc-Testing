using System.Security.Claims;
using FluentValidation;
using KHtmx.Components.Comments;
using KHtmx.Constants;
using KHtmx.Domain.Comments;
using KHtmx.Domain.People;
using KHtmx.Persistence;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KHtmx.Comments;

public static class CommentEndpoints
{
    public static IServiceCollection AddComments(this IServiceCollection services)
    {
        return services;
    }

    public static void MapComments(this IEndpointRouteBuilder route)
    {
        var htmxGroup = route.MapGroup(EndpointConstants.HtmxPrefix);
        htmxGroup.MapPost("Comment", CreateComment.Handler)
            .WithName(CreateComment.EndpointName);

        htmxGroup.MapDelete("Comment/{id}", DeleteComment.Handler)
            .WithName(DeleteComment.EndpointName);

        htmxGroup.MapPatch("Comment/{id}", UpdateComment.Handler)
            .WithName(UpdateComment.EndpointName);

        htmxGroup.MapGet("GetCommentTable", GetCommentTable.Handler)
            .WithName(GetCommentTable.EndpointName);

        htmxGroup.MapGet("GetCommentDialog/{id}", GetCommentDialog.Handler)
            .WithName(GetCommentDialog.EndpointName);

        htmxGroup.MapGet("GetCommentEditForm/{id}/edit", GetCommentEditForm.Handler)
            .WithName(GetCommentEditForm.EndpointName);
    }


    public static class GetCommentTable
    {
        public const string EndpointName = "GetCommentsTable";

        public static RazorComponentResult<CommentTableComponent> Handler
            ()
        {
            return new RazorComponentResult<CommentTableComponent>();
        }
    }

    public static class GetCommentDialog
    {
        public const string EndpointName = "GetCommentDialog";

        public static RazorComponentResult<CommentDialogComponent> Handler
            (Guid id)
        {
            return new RazorComponentResult<CommentDialogComponent>(new
            {
                Id = id
            });
        }
    }

    public static class GetCommentEditForm
    {
        public const string EndpointName = "GetCommentEditForm";

        public static async ValueTask<Results<NotFound, RazorComponentResult<CommentEditFormComponent>>> Handler
        (
            IDbContextFactory<KhDbContext> dbContextFactory,
            Guid id,
            CancellationToken ct
        )
        {
            await using var dbContext = await dbContextFactory.CreateDbContextAsync(ct);
            if (await dbContext.Comments.FirstOrDefaultAsync(x => x.Id == id, ct) is not { } entity)
            {
                return TypedResults.NotFound();
            }

            var dto = CommentEditFormDto.FromCommentEntity(entity);

            return new RazorComponentResult<CommentEditFormComponent>(new
            {
                Comment = dto,
            });
        }
    }

    public static class CreateComment
    {
        public const string EndpointName = "CreateComment";
        private static readonly string[] UserNotFoundError = ["User not found"];

        // TODO: Figure out how to extract authorization logic into a Requirement and Handler
        public static async ValueTask<RazorComponentResult<CommentCreateFormComponent>> Handler
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
                return new RazorComponentResult<CommentCreateFormComponent>(new
                {
                    Comment = dto,
                    Errors = validationResult.Errors.Select(x => x.ErrorMessage).ToArray(),
                });
            }

            await using var dbContext = await dbContextFactory.CreateDbContextAsync(ct);

            if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
            {
                return new RazorComponentResult<CommentCreateFormComponent>(new
                {
                    Comment = dto,
                    Errors = UserNotFoundError,
                });
            }

            DateTimeOffset timestamp = TimeProvider.System.GetUtcNow();
            var entity = Comment.Create(dto.Text, timestamp, user.Id);

            dbContext.Add(entity);
            await dbContext.SaveChangesAsync(ct);

            return new RazorComponentResult<CommentCreateFormComponent>(new
            {
                Comment = new CommentCreateFormDto
                {
                    Text = string.Empty
                },
            });
        }
    }

    public static class DeleteComment
    {
        public const string EndpointName = "DeleteComment";

        public static async ValueTask<Results<NotFound, NoContent, UnauthorizedHttpResult>> Handler
        (
            IDbContextFactory<KhDbContext> dbContextFactory,
            ClaimsPrincipal claimsPrincipal,
            UserManager<KhtmxUser> userManager,
            Guid id,
            CancellationToken ct
        )
        {
            await using var dbContext = await dbContextFactory.CreateDbContextAsync(ct);
            if (await dbContext.Comments.FirstOrDefaultAsync(x => x.Id == id, cancellationToken: ct) is not { } entity)
            {
                return TypedResults.NotFound();
            }

            if (await userManager.GetUserAsync(claimsPrincipal) is not { } user || entity.AuthorId != user.Id)
            {
                return TypedResults.Unauthorized();
            }

            dbContext.Remove(entity);
            await dbContext.SaveChangesAsync(ct);

            return TypedResults.NoContent();
        }
    }

    public static class UpdateComment
    {
        public const string EndpointName = "UpdateComment";

        public static async ValueTask<Results<NotFound, RazorComponentResult<CommentEditFormComponent>, Ok, UnauthorizedHttpResult>> Handler
        (
            IValidator<CommentEditFormDto> validator,
            IDbContextFactory<KhDbContext> dbContextFactory,
            ClaimsPrincipal claimsPrincipal,
            UserManager<KhtmxUser> userManager,
            Guid id,
            [FromForm] CommentEditFormDto dto,
            CancellationToken ct)
        {
            if (await validator.ValidateAsync(dto, ct) is { IsValid: false } validationResult)
            {
                return new RazorComponentResult<CommentEditFormComponent>(new
                {
                    Comment = dto,
                    Errors = validationResult.Errors.Select(x => x.ErrorMessage).ToArray(),
                });
            }

            await using var dbContext = await dbContextFactory.CreateDbContextAsync(ct);
            if (await dbContext.Comments.FirstOrDefaultAsync(x => x.Id == id, cancellationToken: ct) is not { } entity)
            {
                return TypedResults.NotFound();
            }

            if (await userManager.GetUserAsync(claimsPrincipal) is not { } user || entity.AuthorId != user.Id)
            {
                return TypedResults.Unauthorized();
            }

            entity.ChangeText(dto.Text);

            await dbContext.SaveChangesAsync(ct);

            return TypedResults.Ok();
        }
    }
}
