using HRManagement.Api.Features.Employees.Services.Ipml;
using HRManagement.Api.Repositories.Interfaces;

namespace HRManagement.Api.Features.Employees.Services;

public class DepartmentService(
	IDepartmentRepository departmentRepository
) : IDepartmentService
{

}
