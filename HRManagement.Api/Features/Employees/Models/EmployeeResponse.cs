using HRManagement.Api.Constants;

namespace HRManagement.Api.Features.Employees.Models;

public sealed record EmployeeResponse(
	Guid Id,
	string EmployeeCode,
	string FirstName,
	string LastName,
	DateOnly DateOfBirth,
	Gender Gender,
	string? PhoneNumber,
	string? Address,
	DateOnly HireDate,
	EmployeeStatus Status,
	Guid? UserId,
	string? Email,
	DateTime CreatedAtUtc,
	Guid? LastModifiedBy,
	DateTime? LastModifiedAtUtc);
