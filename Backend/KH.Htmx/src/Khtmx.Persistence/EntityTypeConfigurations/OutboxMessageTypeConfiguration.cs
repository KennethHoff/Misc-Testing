using Khtmx.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Khtmx.Persistence.EntityTypeConfigurations;

file sealed class OutboxMessageTypeConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(x => x.Type)
            .HasMaxLength(200);

        builder.Property(x => x.Payload)
            .HasMaxLength(500);
        
        builder.HasIndex(x => x.ProcessedOnUtc);
    }
}
