using KH.Htmx.Domain.People;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KH.Htmx.Persistence.EntityTypeConfigurations;

file sealed class PersonTypeConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.HasKey(e => e.Id);

        builder.ComplexProperty(x => x.Name, name =>
        {
            name.IsRequired();
            name.Property(x => x.First)
                .HasMaxLength(200);

            name.Property(x => x.Last)
                .HasMaxLength(200);
        });

        builder.HasMany(e => e.Comments)
            .WithOne(x => x.Author);
    }
}
