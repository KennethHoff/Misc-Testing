using Khtmx.Domain.Entities;
using MediatR;

namespace Khtmx.Application.Comments.Queries.GetCommentById;

public sealed record class GetCommentByIdQuery(CommentId Id) : IRequest<GetCommentByIdResponse>;
