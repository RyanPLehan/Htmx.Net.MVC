using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ContosoUniversity.Models;

namespace ContosoUniversity.Data.Configurations;

internal sealed class DepartmentConfig : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Department");
        builder.HasKey(x => x.DepartmentID);

        builder.Property<int>(x => x.DepartmentID)
            .HasColumnName("DepartmentID")
            .IsRequired(true)
            .ValueGeneratedOnAdd();

        builder.Property<string>(x => x.Name)
            .HasColumnName("Name")
            .HasMaxLength(50)
            .IsUnicode(false)
            .IsRequired(true);

        builder.Property<decimal>(x => x.Budget)
            .HasColumnName("Budget")
            .HasColumnType("money")
            .IsRequired();

        builder.Property<DateTime>(x => x.StartDate)
            .HasColumnName("StartDate")
            .IsRequired();

        builder.Property<byte[]>(x => x.RowVersion)
            .IsConcurrencyToken()
            .ValueGeneratedOnAddOrUpdate()
            .IsRequired(false);


        builder.HasIndex(x => x.InstructorID)
            .HasDatabaseName<Department>("IX_Department_InstructorID");

        builder.HasOne<Instructor>(x => x.Administrator)
            .WithMany()
            .HasForeignKey(x => x.InstructorID);
    }
}
