using FluentValidation;
using KHtmx.Components.Comments;
using KHtmx.Constants;
using KHtmx.Domain.Comments;
using KHtmx.Persistence;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KHtmx.Comments;

public static class CommentsEndpointExtensions
{
    private const string CommentEndpoint = "comments";

    public static IServiceCollection AddComments(this IServiceCollection services)
    {
        return services;
    }

    public static void MapComments(this IEndpointRouteBuilder route)
    {
        route.MapPost(CommentEndpoint, Api.CreateComment.Handler)
            .WithName(Api.CreateComment.EndpointName);

        route.MapDelete(CommentEndpoint + "/{id}", Api.DeleteComment.Handler)
            .WithName(Api.DeleteComment.EndpointName);

        route.MapPatch(CommentEndpoint + "/{id}", Api.UpdateComment.Handler)
            .WithName(Api.UpdateComment.EndpointName);

        var htmxGroup = route.MapGroup(EndpointConstants.HtmxPrefix);

        htmxGroup.MapGet(CommentEndpoint, Htmx.CommentTable.Handler)
            .WithName(Htmx.CommentTable.EndpointName);

        htmxGroup.MapGet(CommentEndpoint + "/{id}", Htmx.CommentDialog.Handler)
            .WithName(Htmx.CommentDialog.EndpointName);

        htmxGroup.MapGet(CommentEndpoint + "/{id}/edit", Htmx.EditCommentForm.Handler)
            .WithName(Htmx.EditCommentForm.EndpointName);
    }


    public static class Htmx
    {
        public static class CommentTable
        {
            public const string EndpointName = "Htmx_CommentsTable";

            public static RazorComponentResult<CommentTableComponent> Handler
                ()
            {
                return new RazorComponentResult<CommentTableComponent>();
            }
        }

        public static class CommentDialog
        {
            public const string EndpointName = "Htmx_CommentDialog";

            public static RazorComponentResult<CommentDialogComponent> Handler
                (Guid id)
            {
                return new RazorComponentResult<CommentDialogComponent>(new
                {
                    Id = id
                });
            }
        }

        public static class EditCommentForm
        {
            public const string EndpointName = "Htmx_CommentEditForm";

            public static async ValueTask<Results<NotFound, RazorComponentResult<EditCommentFormComponent>>> Handler
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

                var dto = EditCommentFormDto.FromCommentEntity(entity);

                return new RazorComponentResult<EditCommentFormComponent>(new
                {
                    Comment = dto,
                });
            }
        }
    }

    public static class Api
    {
        public static class CreateComment
        {
            public const string EndpointName = "Api_CreateComment";

            public static async ValueTask<RazorComponentResult<CreateCommentFormComponent>> Handler
            (
                IValidator<CreateCommentFormDto> validator,
                IDbContextFactory<KhDbContext> dbContextFactory,
                [FromForm] CreateCommentFormDto dto,
                CancellationToken ct
            )
            {
                if (await validator.ValidateAsync(dto, ct) is { IsValid: false } validationResult)
                {
                    return new RazorComponentResult<CreateCommentFormComponent>(new
                    {
                        Comment = dto,
                        Errors = validationResult.Errors.Select(x => x.ErrorMessage).ToArray(),
                    });
                }

                await using var dbContext = await dbContextFactory.CreateDbContextAsync(ct);

                // TODO: Use CQRS instead, and use the current user
                var user = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == "admin", ct)
                           ?? throw new InvalidOperationException("Admin user not found");

                DateTimeOffset timestamp = TimeProvider.System.GetUtcNow();
                var entity = Comment.Create(dto.Text, timestamp, user.Id);

                dbContext.Add(entity);
                await dbContext.SaveChangesAsync(ct);

                return new RazorComponentResult<CreateCommentFormComponent>(new
                {
                    Comment = dto with
                    {
                        Text = string.Empty,
                    },
                });
            }
        }

        public static class DeleteComment
        {
            public const string EndpointName = "Api_DeleteComment";

            public static async ValueTask<Results<NotFound, NoContent>> Handler
            (
                IDbContextFactory<KhDbContext> dbContextFactory,
                Guid id,
                CancellationToken ct
            )
            {
                await using var dbContext = await dbContextFactory.CreateDbContextAsync(ct);
                if (await dbContext.Comments.FindAsync(id) is not { } entity)
                {
                    return TypedResults.NotFound();
                }

                dbContext.Remove(entity);
                await dbContext.SaveChangesAsync(ct);

                return TypedResults.NoContent();
            }
        }

        public static class UpdateComment
        {
            public const string EndpointName = "Api_UpdateComment";

            public static async ValueTask<Results<NotFound, RazorComponentResult<EditCommentFormComponent>, Ok>> Handler
            (
                IValidator<EditCommentFormDto> validator,
                IDbContextFactory<KhDbContext> dbContextFactory,
                Guid id,
                [FromForm] EditCommentFormDto dto,
                CancellationToken ct)
            {
                if (await validator.ValidateAsync(dto, ct) is { IsValid: false } validationResult)
                {
                    return new RazorComponentResult<EditCommentFormComponent>(new
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

                entity.ChangeText(dto.Text);

                await dbContext.SaveChangesAsync(ct);

                return TypedResults.Ok();
            }
        }
    }
}
