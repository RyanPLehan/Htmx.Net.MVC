using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ContosoUniversity.Models;

namespace ContosoUniversity.Data.Configurations;

internal sealed class InstructorConfig : IEntityTypeConfiguration<Instructor>
{
    public void Configure(EntityTypeBuilder<Instructor> builder)
    {
        builder.ToTable("Person");
        builder.HasBaseType<Person>();

        builder.Property<DateTime>(x => x.HireDate)
            .HasColumnName("HireDate");

        builder.HasDiscriminator()
            .HasValue("Instructor");
    }
}
