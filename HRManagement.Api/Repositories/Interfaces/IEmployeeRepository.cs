using HRManagement.Api.Entities;

namespace HRManagement.Api.Repositories.Interfaces;

public interface IEmployeeRepository
{
	Task AddAsync(Employee employee);
	Task SaveChangesAsync();
}