using HRManagement.Api.Entities;

namespace HRManagement.Api.Repositories.Interfaces;

public interface IDepartmentRepository
{
	Task AddAsync(Department department);

	Task<Department?> GetByIdAsync(Guid id);

	Task<bool> CodeExistsAsync(string departmentCode);

	Task SaveChangesAsync();
}