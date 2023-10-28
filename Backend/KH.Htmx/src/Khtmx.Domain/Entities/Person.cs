using Khtmx.Domain.Primitives;

namespace Khtmx.Domain.Entities;

public sealed class Person : AggregateRoot<Person, PersonId>
{
    private readonly List<Comment> _comments = new();
    
    public Name Name { get; }

    private Person(PersonId id, Name name) : base(id)
    {
        Name = name;
    }
    
    public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();

    public static Person Create(PersonId id, Name name)
    {
        return new Person(id, name);
    }

    public void AddComment(Comment comment)
    {
        RaiseDomainEvent(new CommentAddedToPersonDomainEvent(Id, comment.Id));
        _comments.Add(comment);
    }
}

public readonly record struct PersonId(Guid Value) : ITypedId<PersonId>
{
    public static readonly PersonId Admin = new(Guid.Parse("00000000-0000-0000-0000-000000000001"));
    public static PersonId New() => new(Guid.NewGuid());
    public static PersonId From(Guid value) => new(value);
}
