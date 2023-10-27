using Khtmx.Domain.Entities;
using Khtmx.Domain.Shared;
using MediatR;

namespace Khtmx.Application.Comments.Commands.CreateComment;

public sealed record class CreateCommentCommand(
    PersonId AuthorId,
    PostId PostId,
    string Text,
    DateTimeOffset Timestamp
) : IRequest<Result<CommentId>>;
