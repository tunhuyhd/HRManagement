using HRManagement.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagement.Api.Data.Configurations;

public sealed class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.HasKey(department => department.Id);
        builder.Property(department => department.DepartmentCode).HasMaxLength(30).IsRequired();
        builder.HasIndex(department => department.DepartmentCode)
            .IsUnique()
            .HasFilter("NOT is_deleted");
        builder.Property(department => department.Name).HasMaxLength(150).IsRequired();
        builder.Property(department => department.Description).HasMaxLength(500);
        builder.Property(department => department.IsActive).HasDefaultValue(true);

        builder.HasOne(department => department.ParentDepartment)
            .WithMany(department => department.ChildDepartments)
            .HasForeignKey(department => department.ParentDepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(department => department.ParentDepartmentId);
    }
}
