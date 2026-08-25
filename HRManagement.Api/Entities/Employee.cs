using HRManagement.Api.Constants;

namespace HRManagement.Api.Entities;

public sealed class Employee : BaseEntity
{
    public string EmployeeCode { get; private set; } = null!;

    public string FirstName { get; private set; } = null!;

    public string LastName { get; private set; } = null!;

    public DateOnly DateOfBirth { get; private set; }

    public Gender Gender { get; private set; }

    public string? PhoneNumber { get; private set; }

    public string? Address { get; private set; }

    public DateOnly HireDate { get; private set; }

    public string? Email { get; private set; }

    public EmployeeStatus Status { get; private set; } = EmployeeStatus.Active;

    public Guid? UserId { get; private set; }

    public AppUser? User { get; private set; }

    private Employee()
    {
    }

    public Employee(
        string firstName,
        string lastName,
        DateOnly dateOfBirth,
        Gender gender,
        string? phoneNumber,
        string? address,
        DateOnly hireDate,
        Guid? userId,
        string? email)
    {
        Update(
            firstName,
            lastName,
            dateOfBirth,
            gender,
            phoneNumber ?? string.Empty,
            address ?? string.Empty,
            hireDate,
            email ?? string.Empty,
            EmployeeStatus.Active,
            userId,
            email ?? string.Empty);
    }

    public void Update(
        string firstName,
        string lastName,
        DateOnly dateOfBirth,
        Gender gender,
        string? phoneNumber,
        string? address,
        DateOnly hireDate,
        string email,
        EmployeeStatus status,
        Guid? userId,
        string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        DateOfBirth = dateOfBirth;
        Gender = gender;
        PhoneNumber = phoneNumber?.Trim();
        Address = address?.Trim();
        HireDate = hireDate;
        Email = email.Trim();
        Status = status;
        UserId = userId;
        Email = email.Trim();
    }
}
