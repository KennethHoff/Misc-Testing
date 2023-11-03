using FluentValidation;
using KHtmx.Components.Comments;
using KHtmx.Domain.Comments;
using KHtmx.Persistence;
using Microsoft.AspNetCore.Components.Endpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KHtmx.Comments;

public static class CommentsEndpointExtensions
{
    public const string CommentsEndpoint = "/comments";

    public static IServiceCollection AddComments(this IServiceCollection services)
    {
        return services;
    }

    public static void MapComments(this IEndpointRouteBuilder route)
    {
        route.MapPost(CommentsEndpoint, async ValueTask<RazorComponentResult<CommentForm>> (
                IValidator<CommentFormDto> validator,
                IDbContextFactory<KhDbContext> dbContextFactory,
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

                var entity = dto.ToCommentEntity(TimeProvider.System);

                await using var dbContext = await dbContextFactory.CreateDbContextAsync();

                dbContext.Add(entity);
                await dbContext.SaveChangesAsync();

                return new RazorComponentResult<CommentForm>(new
                {
                    Comment = dto with
                    {
                        Text = string.Empty,
                    },
                });
            })
            .WithName("AddComment");

        route.MapDelete(CommentsEndpoint + "/{id}", async ValueTask<Results<NotFound, NoContent>> (
                IDbContextFactory<KhDbContext> dbContextFactory,
                [FromRoute] CommentId id) =>
            {
                await using var dbContext = await dbContextFactory.CreateDbContextAsync();
                if (await dbContext.Comments.FindAsync(id) is not { } entity)
                {
                    return TypedResults.NotFound();
                }

                dbContext.Remove(entity);
                await dbContext.SaveChangesAsync();

                return TypedResults.NoContent();
            })
            .WithName("DeleteComment");

        route.MapGet(CommentsEndpoint, () => new RazorComponentResult<CommentTable>())
            .WithName("GetComments");

        route.MapGet(CommentsEndpoint + "/{id}", (CommentId id) =>
            {
                return new RazorComponentResult<CommentDialog>(new
                {
                    Id = id
                });
            })
            .WithName("GetComment");

        // TODO: Fix this to return a form that edits instead of creates
        route.MapGet(CommentsEndpoint + "/{id}/edit",
                async ValueTask<Results<NotFound, RazorComponentResult<CommentForm>>> (
                    IDbContextFactory<KhDbContext> dbContextFactory,
                    CommentId id
                ) =>
                {
                    await using var dbContext = await dbContextFactory.CreateDbContextAsync();
                    if (await dbContext.Comments.FindAsync(id) is not { } entity)
                    {
                        return TypedResults.NotFound();
                    }

                    var dto = CommentFormDto.FromCommentEntity(entity);

                    return new RazorComponentResult<CommentForm>(new
                    {
                        Comment = dto,
                        IsEdit = true
                    });
                })
            .WithName("GetEditComment");
    }
}
