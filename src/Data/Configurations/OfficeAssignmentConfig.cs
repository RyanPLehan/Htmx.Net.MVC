using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ContosoUniversity.Models;

namespace ContosoUniversity.Data.Configurations;

internal sealed class OfficeAssignmentConfig : IEntityTypeConfiguration<OfficeAssignment>
{
    public void Configure(EntityTypeBuilder<OfficeAssignment> builder)
    {
        builder.ToTable("OfficeAssignment");
        builder.HasKey(x => x.InstructorID);

        builder.Property<int>(x => x.InstructorID)
            .HasColumnName("InstructorID")
            .IsRequired(true);

        builder.Property<string>(x => x.Location)
            .HasColumnName("Location")
            .HasMaxLength(50)
            .IsUnicode(false)
            .IsRequired(true);

        builder.HasOne<Instructor>(x => x.Instructor)
            .WithOne(x => x.OfficeAssignment)
            .HasForeignKey<OfficeAssignment>(x => x.InstructorID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
