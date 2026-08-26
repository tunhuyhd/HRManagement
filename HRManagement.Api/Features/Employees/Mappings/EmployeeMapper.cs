using HRManagement.Api.Entities;
using HRManagement.Api.Features.Employees.Models;

namespace HRManagement.Api.Features.Employees.Mappings;

public static class EmployeeMapper
{
	public static EmployeeResponse ToResponse(this Employee employee) =>
		new(
			employee.Id,
			employee.EmployeeCode,
			employee.FirstName,
			employee.LastName,
			employee.DateOfBirth,
			employee.Gender,
			employee.PhoneNumber ?? string.Empty,
			employee.Address ?? string.Empty,
			employee.HireDate,
			employee.Status,
			employee.UserId,
			employee.Email ?? string.Empty,
			employee.CreatedAtUtc,
			employee.LastModifiedBy,
			employee.LastModifiedAtUtc);
}
