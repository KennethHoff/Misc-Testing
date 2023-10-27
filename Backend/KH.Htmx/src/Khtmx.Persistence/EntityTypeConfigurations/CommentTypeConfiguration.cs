using Khtmx.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Khtmx.Persistence.EntityTypeConfigurations;

file sealed class CommentTypeConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Text)
            .HasMaxLength(500);

        builder.Property(e => e.Timestamp);
    }
}
