using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ContosoUniversity.Models;

namespace ContosoUniversity.Data.Configurations;

internal sealed class CourseAssignmentConfig : IEntityTypeConfiguration<CourseAssignment>
{
    public void Configure(EntityTypeBuilder<CourseAssignment> builder)
    {
        builder.ToTable("CourseAssignment");
        builder.HasKey(x => new { x.CourseID, x.InstructorID });

        builder.Property<int>(x => x.CourseID)
            .HasColumnName("CourseID")
            .IsRequired(true);

        builder.Property<int>(x => x.InstructorID)
            .HasColumnName("InstructorID")
            .IsRequired(true);

        builder.HasIndex(x => x.InstructorID)
            .HasDatabaseName<CourseAssignment>("IX_CourseAssignment_InstructorID");

        builder.HasOne<Course>(x => x.Course)
            .WithMany(x => x.CourseAssignments)
            .HasForeignKey(x => x.CourseID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Instructor>(x => x.Instructor)
            .WithMany(x => x.CourseAssignments)
            .HasForeignKey(x => x.InstructorID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
