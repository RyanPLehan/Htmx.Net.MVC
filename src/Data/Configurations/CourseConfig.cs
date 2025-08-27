using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ContosoUniversity.Models;

namespace ContosoUniversity.Data.Configurations;

internal sealed class CourseConfig : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Course");
        builder.HasKey(x => x.CourseID);

        builder.Property<int>(x => x.CourseID)
            .HasColumnName("CourseID")
            .IsRequired(true)
            .ValueGeneratedNever();

        builder.Property<int>(x => x.Credits)
            .HasColumnName("Credits")
            .IsRequired(true);

        builder.Property<int>(x => x.DepartmentID)
            .HasColumnName("DepartmentID")
            .IsRequired(true);

        builder.Property<string>("Title")
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.HasIndex(x => x.DepartmentID)
            .HasDatabaseName<Course>("IX_Course_DepartmentID");

        builder.HasOne<Department>(x => x.Department)
            .WithMany(x => x.Courses)
            .HasForeignKey(x => x.DepartmentID)
            .OnDelete(DeleteBehavior.Cascade);


    }
}
