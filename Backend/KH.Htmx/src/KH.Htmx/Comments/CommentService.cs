namespace KH.Htmx.Comments;

public sealed class CommentService
{
    private readonly List<string> _comments = new();
    
    public void AddComment(string comment)
    {
        _comments.Add(comment);
    }
    
    public IReadOnlyList<string> GetComments()
    {
        return _comments.AsReadOnly();
    }
}
