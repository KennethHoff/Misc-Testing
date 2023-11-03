using KHtmx.Domain.Comments;
using KHtmx.Domain.People;
using KHtmx.Domain.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace KHtmx.Persistence;

public sealed class KhDbContext(DbContextOptions<KhDbContext> options) : DbContext(options)
{
    public DbSet<Comment> Comments { get; init; } = null!;
    public DbSet<Person> People { get; init; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(KhDbContext).Assembly);
    }
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<PersonId>().HaveConversion<TypedIdEfCoreValueConverter<PersonId>>();
        configurationBuilder.Properties<CommentId>().HaveConversion<TypedIdEfCoreValueConverter<CommentId>>();
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
