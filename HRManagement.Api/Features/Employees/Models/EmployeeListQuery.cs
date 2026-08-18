using System.ComponentModel.DataAnnotations;

namespace HRManagement.Api.Features.Employees.Models;

public sealed class EmployeeListQuery
{
	[Range(1, int.MaxValue)]
	public int PageNumber { get; init; } = 1;

	[Range(1, 100)]
	public int PageSize { get; init; } = 20;

	[MaxLength(100)]
	public string? Search { get; init; }
}