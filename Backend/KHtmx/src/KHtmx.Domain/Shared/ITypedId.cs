namespace KHtmx.Domain.Shared;

public interface ITypedId<out TSelf>
    where TSelf : ITypedId<TSelf>
{
    Guid Value { get; }
    static abstract TSelf From(Guid value);
    static abstract TSelf New();
}
