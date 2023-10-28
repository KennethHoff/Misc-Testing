using Khtmx.Domain.Entities;

namespace Khtmx.Domain.Abstractions;

public interface ICommentRepository
{
    void Insert(Comment comment);
}
