using KHtmx.Domain.Comments;
using KHtmx.Domain.People;
using KHtmx.Domain.Posts;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KHtmx.Persistence;

public sealed class KhDbContext(DbContextOptions<KhDbContext> options) : IdentityDbContext<KhtmxUser, KhtmxRole, Guid>(options)
{
    public DbSet<Comment> Comments { get; init; } = null!;
    public DbSet<Post> Posts { get; init; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(KhDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
