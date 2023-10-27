using Khtmx.Domain.Entities;

namespace Khtmx.Domain.Exceptions;

public sealed class CommentNotFoundException(CommentId commentId) : Exception($"Comment with id {commentId} was not found.");
