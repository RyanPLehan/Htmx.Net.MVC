using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ContosoUniversity.Models;

namespace ContosoUniversity.Data.Configurations;

internal sealed class PersonConfig : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("Person");
        builder.HasKey(x => x.ID);
        builder.Ignore(x => x.FullName);

        builder.Property<int>(x => x.ID)
            .HasColumnName("ID")
            .IsRequired(true)
            .ValueGeneratedOnAdd();

        builder.Property<string>(x => x.FirstName)
            .HasColumnName("FirstName")
            .HasMaxLength(50)
            .IsRequired()
            .IsUnicode(true);

        builder.Property<string>(x => x.LastName)
            .HasColumnName("LastName")
            .HasMaxLength(50)
            .IsRequired()
            .IsUnicode(true);

        builder.HasDiscriminator<string>("Discriminator")
            .HasValue("Person");
    }
}
