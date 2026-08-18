using System.ComponentModel.DataAnnotations;

namespace HRManagement.Api.Features.Employees.Models;

internal static class EmployeeRequestValidator
{
    public static IEnumerable<ValidationResult> Validate(
        string firstName,
        string lastName,
        DateOnly dateOfBirth,
        DateOnly hireDate)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            yield return new ValidationResult(
                "First name must not be empty or whitespace.",
                [nameof(CreateEmployeeRequest.FirstName)]);
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            yield return new ValidationResult(
                "Last name must not be empty or whitespace.",
                [nameof(CreateEmployeeRequest.LastName)]);
        }

        if (dateOfBirth >= DateOnly.FromDateTime(DateTime.UtcNow))
        {
            yield return new ValidationResult(
                "Date of birth must be before today.",
                [nameof(CreateEmployeeRequest.DateOfBirth)]);
        }

        if (hireDate < dateOfBirth)
        {
            yield return new ValidationResult(
                "Hire date must not be before date of birth.",
                [nameof(CreateEmployeeRequest.HireDate)]);
        }
    }
}
