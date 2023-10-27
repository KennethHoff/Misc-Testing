using MediatR;

namespace Khtmx.Application.Comments.Queries.GetCommentById;

public sealed class GetCommentByIdQueryHandler : IRequestHandler<GetCommentByIdQuery, GetCommentByIdResponse>
{
    public Task<GetCommentByIdResponse> Handle(GetCommentByIdQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
