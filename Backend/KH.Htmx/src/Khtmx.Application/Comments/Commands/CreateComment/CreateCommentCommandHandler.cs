using Khtmx.Domain.Abstractions;
using Khtmx.Domain.Entities;
using Khtmx.Domain.Errors;
using Khtmx.Domain.Shared;
using MediatR;

namespace Khtmx.Application.Comments.Commands.CreateComment;

public sealed class CreateCommentCommandHandler(
    IPersonRepository personRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<CreateCommentCommand, Result<CommentId>>
{
    public async Task<Result<CommentId>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var commentResult = Comment.Create(CommentId.New(), request.AuthorId, request.Text, request.Timestamp);
        if (commentResult.IsFailure)
        {
            return Result.Failure<CommentId>(commentResult.Error);
        }
        
        if (await personRepository.GetByIdAsync(request.AuthorId, cancellationToken) is not {} person)
        {
            return Result.Failure<CommentId>(DomainErrors.Person.NotFound);
        }
        
        person.AddComment(commentResult.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return commentResult.Id;
    }
}
