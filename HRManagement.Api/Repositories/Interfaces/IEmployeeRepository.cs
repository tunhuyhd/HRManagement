using HRManagement.Api.Entities;

namespace HRManagement.Api.Repositories.Interfaces;

public interface IEmployeeRepository
{
	Task AddAsync(Employee employee);
	Task<IReadOnlyList<Employee>> GetListAsync();
	Task<Employee?> GetByIdAsync(Guid id);
	Task SaveChangesAsync();
}