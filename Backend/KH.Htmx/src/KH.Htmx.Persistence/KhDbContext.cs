using KH.Htmx.Domain.Comments;
using KH.Htmx.Domain.People;
using KH.Htmx.Domain.Primitives;
using KH.Htmx.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace KH.Htmx.Persistence;

public sealed class KhDbContext(DbContextOptions<KhDbContext> options) : DbContext(options)
{
    public DbSet<Comment> Comments { get; init; } = null!;
    public DbSet<Person> People { get; init; } = null!;
    public DbSet<OutboxMessage> OutboxMessages { get; init; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(KhDbContext).Assembly);
    }
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<PersonId>().HaveConversion<TypedIdEfCoreValueConverter<PersonId>>();
        configurationBuilder.Properties<CommentId>().HaveConversion<TypedIdEfCoreValueConverter<CommentId>>();
        configurationBuilder.Properties<OutboxMessageId>().HaveConversion<TypedIdEfCoreValueConverter<OutboxMessageId>>();
    }
}

file sealed class TypedIdEfCoreValueConverter<TId>() : ValueConverter<TId, Guid>(v => v.Value, v => Create(v))
    where TId : ITypedId<TId>
{
    private static TId Create(Guid value)
    {
        var id = TId.From(value);
        return id;
    }
}
