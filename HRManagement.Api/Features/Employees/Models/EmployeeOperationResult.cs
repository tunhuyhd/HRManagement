namespace HRManagement.Api.Features.Employees.Models;

public sealed record EmployeeOperationResult(
    EmployeeResponse? Employee,
    EmployeeOperationError Error = EmployeeOperationError.None)
{
    public bool IsSuccess => Error == EmployeeOperationError.None;

    public static EmployeeOperationResult Success(EmployeeResponse employee) => new(employee);

    public static EmployeeOperationResult Failure(EmployeeOperationError error) => new(null, error);
}
