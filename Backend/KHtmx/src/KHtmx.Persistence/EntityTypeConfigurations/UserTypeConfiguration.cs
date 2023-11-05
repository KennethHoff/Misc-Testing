using KHtmx.Domain.People;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KHtmx.Persistence.EntityTypeConfigurations;

file sealed class UserTypeConfiguration : IEntityTypeConfiguration<KhtmxUser>
{
    public void Configure(EntityTypeBuilder<KhtmxUser> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirstName)
            .HasMaxLength(50);

        builder.Property(u => u.LastName)
            .HasMaxLength(50);
    }
}
