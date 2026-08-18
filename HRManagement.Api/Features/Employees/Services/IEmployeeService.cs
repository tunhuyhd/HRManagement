using HRManagement.Api.Features.Employees.Models;

namespace HRManagement.Api.Features.Employees.Services;

public interface IEmployeeService
{
	Task<EmployeeResponse?> CreateAsync(CreateEmployeeRequest request);
}