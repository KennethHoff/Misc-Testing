namespace Khtmx.Domain.Primitives;

public interface IAggregateRoot
{
    IReadOnlyCollection<IDomainEvent> GetDomainEvents();
    void ClearDomainEvents();
}

public abstract class AggregateRoot<TSelf, TId>(TId id) : Entity<TSelf, TId>(id), IAggregateRoot
    where TSelf : AggregateRoot<TSelf, TId>
    where TId : ITypedId<TId>
{
    private readonly List<IDomainEvent> _domainEvents = new();

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// This has to return a copy of the domain events list so that it can be cleared
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> GetDomainEvents() => _domainEvents.ToArray();

    public void ClearDomainEvents() => _domainEvents.Clear();
}
