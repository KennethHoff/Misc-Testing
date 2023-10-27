namespace Khtmx.Domain.Primitives;

public abstract class Entity<TSelf, TId>(TId id) : IEquatable<TSelf>
    where TSelf : Entity<TSelf, TId>
    where TId: ITypedId<TId>
{
    public TId Id { get; } = id;

    public bool Equals(TSelf? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return EqualityComparer<TId>.Default.Equals(Id, other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != this.GetType())
        {
            return false;
        }

        return Equals((TSelf)obj);
    }

    public override int GetHashCode()
    {
        return EqualityComparer<TId>.Default.GetHashCode(Id);
    }

    public static bool operator ==(Entity<TSelf, TId> left, TSelf? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Entity<TSelf, TId> left, TSelf? right)
    {
        return !Equals(left, right);
    }
}
