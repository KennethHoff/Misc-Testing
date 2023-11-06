using KHtmx.Domain.Comments;

namespace KHtmx.Comments;

public interface ICommentTableFilter
{
    IQueryable<Comment> Apply(IQueryable<Comment> query);

    QueryString QueryString { get; }
}

public sealed class AuthorCommentTableFilter(Guid authorId) : ICommentTableFilter
{
    public IQueryable<Comment> Apply(IQueryable<Comment> query)
    {
        return query.Where(x => x.AuthorId == authorId);
    }

    public QueryString QueryString => QueryString.Create("authorId", authorId.ToString());
}
