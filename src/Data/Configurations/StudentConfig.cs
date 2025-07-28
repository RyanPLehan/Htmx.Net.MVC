using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ContosoUniversity.Models;

namespace ContosoUniversity.Data.Configurations;

internal sealed class StudentConfig : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Person");
        builder.HasBaseType<Person>();

        builder.Property<DateTime>(x => x.EnrollmentDate)
            .HasColumnName("EnrollmentDate");

        builder.HasDiscriminator()
            .HasValue("Student");
    }
}
