using HRManagement.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagement.Api.Data.Configurations;

public sealed class EmployeeConfiguration
	: IEntityTypeConfiguration<Employee>
{
	public void Configure(EntityTypeBuilder<Employee> builder)
	{
		builder.ToTable("Employees");

		builder.HasKey(employee => employee.Id);

		builder.Property(employee => employee.EmployeeCode)
			.HasMaxLength(20)
			.HasDefaultValueSql(
				"'EMP' || LPAD(nextval('employee_code_sequence')::text, 3, '0')")
			.ValueGeneratedOnAdd();

		builder.HasIndex(employee => employee.EmployeeCode)
			.IsUnique();

		builder.Property(employee => employee.FirstName)
			.HasMaxLength(100)
			.IsRequired();

		builder.Property(employee => employee.LastName)
			.HasMaxLength(100)
			.IsRequired();

		builder.Property(employee => employee.PhoneNumber)
			.HasMaxLength(20);

		builder.Property(employee => employee.Address)
			.HasMaxLength(500);

		builder.Property(employee => employee.Status)
			.HasConversion<string>()
			.HasMaxLength(30);

		builder.Property(employee => employee.Gender)
			.HasConversion<string>()
			.HasMaxLength(20);

		builder.HasOne(employee => employee.User)
			.WithOne()
			.HasForeignKey<Employee>(employee => employee.UserId)
			.OnDelete(DeleteBehavior.SetNull);
	}
}