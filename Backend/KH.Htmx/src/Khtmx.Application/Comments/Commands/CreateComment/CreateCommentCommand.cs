using Khtmx.Domain.Entities;
using MediatR;

namespace Khtmx.Application.Comments.Commands.CreateComment;

public sealed record class CreateCommentCommand(
    PersonId AuthorId,
    string Text,
    DateTimeOffset Timestamp
) : IRequest<CommentId>;
