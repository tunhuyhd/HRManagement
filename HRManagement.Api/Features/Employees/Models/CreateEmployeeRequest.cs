using System.ComponentModel.DataAnnotations;
using HRManagement.Api.Constants;

namespace HRManagement.Api.Features.Employees.Models;

public sealed record CreateEmployeeRequest(
	[Required, MaxLength(100)] string FirstName,
	[Required, MaxLength(100)] string LastName,
	DateOnly DateOfBirth,
	Gender Gender,
	[MaxLength(20)] string? PhoneNumber,
	[MaxLength(500)] string? Address,
	DateOnly HireDate,
	Guid? UserId);