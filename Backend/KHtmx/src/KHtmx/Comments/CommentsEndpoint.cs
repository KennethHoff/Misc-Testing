using FluentValidation;
using KHtmx.Components.Comments;
using KHtmx.Domain.Comments;
using KHtmx.Persistence;
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
        route.MapPost(CommentsEndpoint, async Task<RazorComponentResult<CommentForm>> (
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

            return new RazorComponentResult<CommentForm>();
        });

        route.MapGet(CommentsEndpoint, () => new RazorComponentResult<CommentTable>());
        route.MapGet(CommentsEndpoint + "/{id}", ([FromRoute] CommentId id) =>
        {
            return new RazorComponentResult<CommentDialog>(new { Id = id });
        });
    }
}
