using System.Diagnostics.Metrics;
using System.Security.Claims;
using FluentValidation;
using KHtmx.Components.Comments.Data;
using KHtmx.Constants;
using KHtmx.Domain.Comments;
using KHtmx.Domain.People;
using KHtmx.Models;
using KHtmx.Persistence;
using KHtmx.Telemetry;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KHtmx.Components.Comments;

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


    public sealed class GetCommentTable
    {
        public const string EndpointName = "GetCommentsTable";

        public static RazorComponentResult<CommentTableComponent> Handler
        (
            [FromQuery(Name = "authorId")] Guid? authorId
        )
        {
            if (authorId is { } authorGuid)
            {
                return new RazorComponentResult<CommentTableComponent>(new
                {
                    Filter = new AuthorCommentTableFilter(authorGuid),
                });
            }

            return new RazorComponentResult<CommentTableComponent>();
        }
    }

    public sealed class GetCommentDialog
    {
        public const string EndpointName = "GetCommentDialog";

        public static RazorComponentResult<CommentDialogComponent> Handler
        (
            Guid id
        )
        {
            return new RazorComponentResult<CommentDialogComponent>(new
            {
                Id = id
            });
        }
    }

    public sealed class GetCommentEditForm
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
                FormData = dto,
            });
        }
    }

    public sealed class CreateComment
    {
        public const string EndpointName = "CreateComment";

        // TODO: Figure out how to extract authorization logic into a Requirement and Handler
        public static async ValueTask<RazorComponentResult<CommentCreateFormComponent>> Handler
        (
            IValidator<CommentCreateFormDto> validator,
            IDbContextFactory<KhDbContext> dbContextFactory,
            ClaimsPrincipal claimsPrincipal,
            UserManager<KhtmxUser> userManager,
            [FromForm] CommentCreateFormDto dto,
            CancellationToken ct
        )
        {
            if (await validator.ValidateAsync(dto, ct) is { IsValid: false } validationResult)
            {
                return new RazorComponentResult<CommentCreateFormComponent>(new
                {
                    FormData = dto,
                    ValidationFailures = validationResult.Errors.ToValidationFailures()
                });
            }

            await using var dbContext = await dbContextFactory.CreateDbContextAsync(ct);

            if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
            {
                return new RazorComponentResult<CommentCreateFormComponent>(new
                {
                    FormData = dto,
                    ValidationFailures = new ValidationFailureCollection
                    {
                        new()
                        {
                            ErrorMessage = "You must be logged in to comment"
                        }
                    }
                });
            }

            DateTimeOffset timestamp = TimeProvider.System.GetUtcNow();
            var entity = Comment.Create(dto.Text, timestamp, user.Id);

            dbContext.Add(entity);
            await dbContext.SaveChangesAsync(ct);

            return new RazorComponentResult<CommentCreateFormComponent>(new
            {
                FormData = new CommentCreateFormDto
                {
                    Text = string.Empty
                },
            });
        }
    }

    public sealed class DeleteComment
    {
        public const string EndpointName = "DeleteComment";

        public static async ValueTask<Results<NotFound, NoContent, UnauthorizedHttpResult>> Handler
        (
            IDbContextFactory<KhDbContext> dbContextFactory,
            ClaimsPrincipal claimsPrincipal,
            UserManager<KhtmxUser> userManager,
            Guid id,
            IMeterFactory meterFactory,
            CancellationToken ct
        )
        {
            await using var dbContext = await dbContextFactory.CreateDbContextAsync(ct);
            if (await dbContext.Comments.FirstOrDefaultAsync(x => x.Id == id, cancellationToken: ct) is not { } entity)
            {
                meterFactory.Create(MetricNames.CommentDeleteFailedNotFound)
                    .CreateCounter<long>(MetricNames.CommentDeleteFailedNotFound)
                    .Add(1, tags:
                    [
                        KeyValuePair.Create<string, object?>("comment_id", id),
                        KeyValuePair.Create<string, object?>("user_username", claimsPrincipal.Identity?.Name)
                    ]);

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

    public sealed class UpdateComment
    {
        public const string EndpointName = "UpdateComment";

        public static async ValueTask<Results<NotFound, RazorComponentResult<CommentEditFormComponent>, Ok, UnauthorizedHttpResult>> Handler
        (
            IValidator<CommentEditFormDto> validator,
            IDbContextFactory<KhDbContext> dbContextFactory,
            ClaimsPrincipal claimsPrincipal,
            UserManager<KhtmxUser> userManager,
            Guid id,
            IMeterFactory meterFactory,
            [FromForm] CommentEditFormDto dto,
            CancellationToken ct)
        {
            Console.WriteLine("Updating comment {0} by user {1}", id, claimsPrincipal.Identity?.Name);
            if (await validator.ValidateAsync(dto, ct) is { IsValid: false } validationResult)
            {
                meterFactory.Create(MetricNames.CommentEditFailedValidation)
                    .CreateCounter<long>(MetricNames.CommentEditFailedValidation)
                    .Add(1);

                Console.WriteLine("Validation failed for comment {0} by user {1}", id, claimsPrincipal.Identity?.Name);
                return new RazorComponentResult<CommentEditFormComponent>(new
                {
                    FormData = dto,
                    ValidationFailures = validationResult.Errors.ToValidationFailures(),
                });
            }

            await using var dbContext = await dbContextFactory.CreateDbContextAsync(ct);
            if (await dbContext.Comments.FirstOrDefaultAsync(x => x.Id == id, cancellationToken: ct) is not { } comment)
            {
                return TypedResults.NotFound();
            }

            if (await userManager.GetUserAsync(claimsPrincipal) is not { } user || comment.AuthorId != user.Id)
            {
                return TypedResults.Unauthorized();
            }

            comment.ChangeText(dto.Text);

            await dbContext.SaveChangesAsync(ct);

            return TypedResults.Ok();
        }
    }
}
