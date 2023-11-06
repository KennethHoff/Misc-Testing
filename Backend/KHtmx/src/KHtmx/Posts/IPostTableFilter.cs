using KHtmx.Domain.Posts;

namespace KHtmx.Posts;

public interface IPostTableFilter
{
    IQueryable<Post> Apply(IQueryable<Post> query);

    QueryString QueryString { get; }
}
