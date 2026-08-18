using HRManagement.Api.Entities;

namespace HRManagement.Api.Repositories.Interfaces;

public interface IEmployeeRepository
{
	Task AddAsync(Employee employee);
	Task<Employee?> GetByIdAsync(Guid id);
	Task<Employee?> GetByIdForUpdateAsync(Guid id);
	Task<bool> IsUserAssignedAsync(Guid userId, Guid? excludedEmployeeId = null);
	void Remove(Employee employee);
	Task SaveChangesAsync();
	Task<(IReadOnlyList<Employee> Items, int TotalCount)> GetListAsync(int pageNumber, int pageSize, string? search);
}
