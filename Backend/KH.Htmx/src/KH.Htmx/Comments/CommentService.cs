using System.Diagnostics.Metrics;
using KH.Htmx.Comments.Events;
using KH.Htmx.Constants;
using MediatR;

namespace KH.Htmx.Comments;

public interface ICommentService
{
    void AddComment(Comment comment);
    IReadOnlyList<Comment> GetComments();
}

internal sealed class CommentService(
    IMediator mediator,
    IMeterFactory meterFactory
) : ICommentService
{
    private readonly List<Comment> _comments = new();

    private readonly Meter _commentsAdded = meterFactory.Create(new MeterOptions(MetricNames.CommentsAdded));

    public void AddComment(Comment comment)
    {
        _commentsAdded.CreateCounter<long>(MetricNames.CommentsAdded).Add(1);
        mediator.Publish(new CommentAddedEvent(comment));
        _comments.Add(comment);
    }

    public IReadOnlyList<Comment> GetComments()
    {
        return _comments.TakeLast(10).Reverse().ToArray().AsReadOnly();
    }
}
