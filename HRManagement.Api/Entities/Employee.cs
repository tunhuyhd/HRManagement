using HRManagement.Api.Constants;

namespace HRManagement.Api.Entities;

public sealed class Employee : BaseEntity
{
    public string EmployeeCode { get; private set; } = null!;

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public Gender Gender { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public DateOnly HireDate { get; set; }

    public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;

    public Guid? UserId { get; set; }

    public AppUser? User { get; set; }
}
