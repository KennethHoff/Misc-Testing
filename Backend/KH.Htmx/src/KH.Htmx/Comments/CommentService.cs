using KH.Htmx.Comments.Events;
using MediatR;

namespace KH.Htmx.Comments;

public interface ICommentService
{
    void AddComment(Comment comment);
    IReadOnlyList<Comment> GetComments();
}

internal sealed class CommentService(IMediator mediator) : ICommentService
{
    private readonly List<Comment> _comments = new();
    
    public void AddComment(Comment comment)
    {
        mediator.Publish(new CommentAddedEvent(comment));
        _comments.Add(comment);
    }
    
    public IReadOnlyList<Comment> GetComments()
    {
        return _comments.TakeLast(10).Reverse().ToArray().AsReadOnly();
    }
}
