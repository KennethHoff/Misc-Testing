using KHtmx.Domain.People;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KHtmx.Persistence.EntityTypeConfigurations;

file sealed class PersonTypeConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.HasKey(e => e.Id);

        builder.ComplexProperty(x => x.Name);

        builder.HasMany(e => e.Comments)
            .WithOne(x => x.Author);
    }
}