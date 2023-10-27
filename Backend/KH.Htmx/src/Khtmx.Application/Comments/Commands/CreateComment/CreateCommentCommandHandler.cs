using Khtmx.Domain.Abstractions;
using Khtmx.Domain.Entities;
using MediatR;

namespace Khtmx.Application.Comments.Commands.CreateComment;

public sealed class CreateCommentCommandHandler(
    ICommentRepository commentRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<CreateCommentCommand, CommentId>
{
    public async Task<CommentId> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = new Comment
        {
            Text = request.Text,
            Timestamp = request.Timestamp,
            AuthorId = request.AuthorId,
        };

        commentRepository.Insert(comment);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return comment.Id;
    }
}
