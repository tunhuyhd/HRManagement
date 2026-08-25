using System.ComponentModel.DataAnnotations;
using HRManagement.Api.Constants;

namespace HRManagement.Api.Features.Employees.Models;

public sealed record UpdateEmployeeRequest(
    [Required, MaxLength(100)] string FirstName,
    [Required, MaxLength(100)] string LastName,
    DateOnly DateOfBirth,
    [EnumDataType(typeof(Gender))] Gender Gender,
    [MaxLength(20)] string? PhoneNumber,
    [MaxLength(500)] string? Address,
    DateOnly HireDate,
    [MaxLength(500)] string Email,
    [EnumDataType(typeof(EmployeeStatus))] EmployeeStatus Status,
    Guid? UserId) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) =>
        EmployeeRequestValidator.Validate(FirstName, LastName, DateOfBirth, HireDate);
}
