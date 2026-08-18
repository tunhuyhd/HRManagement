using HRManagement.Api.Common.Pagination;
using HRManagement.Api.Features.Employees.Models;
namespace HRManagement.Api.Features.Employees.Services;

public interface IEmployeeService
{
	Task<EmployeeOperationResult> CreateAsync(CreateEmployeeRequest request);

	Task<EmployeeResponse?> GetByIdAsync(Guid id);

	Task<EmployeeOperationResult> UpdateAsync(Guid id, UpdateEmployeeRequest request);

	Task<bool> DeleteAsync(Guid id);

	Task<PagedResponse<EmployeeResponse>> GetListAsync(EmployeeListQuery query);
}
