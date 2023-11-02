namespace KHtmx.Domain.Primitives;

public interface ITypedId<TSelf> : IParsable<TSelf>
    where TSelf : ITypedId<TSelf>
{
    Guid Value { get; }
    static abstract TSelf From(Guid value);
    static abstract TSelf New();
}
