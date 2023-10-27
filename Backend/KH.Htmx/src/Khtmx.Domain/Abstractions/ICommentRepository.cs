using Khtmx.Domain.Entities;

namespace Khtmx.Domain.Abstractions;

public interface ICommentRepository
{
    void Insert(Comment comment);
}

public interface IPersonRepository
{
    Task<Person?> GetByIdAsync(PersonId id, CancellationToken cancellationToken);
}
