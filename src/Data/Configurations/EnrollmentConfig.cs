using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ContosoUniversity.Models;

namespace ContosoUniversity.Data.Configurations;

internal sealed class EnrollmentConfig : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("Enrollment");
        builder.HasKey(x => x.EnrollmentID);

        builder.Property<int>(x => x.EnrollmentID)
            .HasColumnName("EnrollmentID")
            .IsRequired(true)
            .ValueGeneratedOnAdd();

        builder.Property<int>(x => x.CourseID)
            .HasColumnName("CourseID")
            .IsRequired(true);

        builder.Property<int>(x => x.StudentID)
            .HasColumnName("StudentID")
            .IsRequired(true);

        builder.Property<Grade?>(x => x.Grade)
            .HasColumnName("Grade")
            .IsRequired(false);

        builder.HasIndex(x => x.StudentID)
            .HasDatabaseName<Enrollment>("IX_Enrollment_StudentID");

        builder.HasIndex(x => x.CourseID)
            .HasDatabaseName<Enrollment>("IX_Enrollment_CourseID");

        builder.HasOne<Course>(x => x.Course)
            .WithMany(x => x.Enrollments)
            .HasForeignKey(x => x.CourseID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Student>(x => x.Student)
            .WithMany(x => x.Enrollments)
            .HasForeignKey(x => x.StudentID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
