using HRManagement.Api.Constants;
using HRManagement.Api.Entities;

namespace HRManagement.Api.Tests.Entities;

public sealed class EmployeeTests
{
    [Fact]
    public void Constructor_NormalizesTextAndStartsActive()
    {
        var userId = Guid.NewGuid();
        var employee = new Employee(
            "  An  ", "  Nguyen  ", new DateOnly(1995, 5, 10), Gender.Female,
            "  0901234567  ", "  Ho Chi Minh City  ",
            new DateOnly(2024, 1, 15), userId, " annguyen123@gmail.com ");

        Assert.Equal("An", employee.FirstName);
        Assert.Equal("Nguyen", employee.LastName);
        Assert.Equal("0901234567", employee.PhoneNumber);
        Assert.Equal("Ho Chi Minh City", employee.Address);
        Assert.Equal(EmployeeStatus.Active, employee.Status);
        Assert.Equal(userId, employee.UserId);
    }

    [Fact]
    public void Update_ChangesProfileAndStatus()
    {
        var employee = CreateEmployee();

        employee.Update(
            "Binh", "Tran", new DateOnly(1990, 2, 20), Gender.Male,
            null, null, new DateOnly(2020, 3, 1), EmployeeStatus.OnLeave, null, string.Empty);

        Assert.Equal("Binh", employee.FirstName);
        Assert.Equal("Tran", employee.LastName);
        Assert.Equal(EmployeeStatus.OnLeave, employee.Status);
        Assert.Null(employee.UserId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_RejectsBlankFirstName(string firstName)
    {
        Assert.Throws<ArgumentException>(() => new Employee(
            firstName, "Nguyen", new DateOnly(1995, 5, 10), Gender.Female,
            null, null, new DateOnly(2024, 1, 15), null, " annguyen123@gmail.com "));
    }

    private static Employee CreateEmployee() => new(
        "An", "Nguyen", new DateOnly(1995, 5, 10), Gender.Female,
        null, null, new DateOnly(2024, 1, 15), Guid.NewGuid(), " annguyen123@gmail.com ");
}

public sealed class PasswordResetRequestTests
{
    [Fact]
    public void Complete_StoresCompletionDetails()
    {
        var request = new PasswordResetRequest(Guid.NewGuid());
        var completedBy = Guid.NewGuid();
        var completedAtUtc = new DateTime(2026, 8, 20, 10, 30, 0, DateTimeKind.Utc);

        request.Complete(completedBy, completedAtUtc);

        Assert.Equal(PasswordResetStatus.Completed, request.Status);
        Assert.Equal(completedBy, request.CompletedBy);
        Assert.Equal(completedAtUtc, request.CompletedAtUtc);
    }

    [Fact]
    public void Complete_WhenAlreadyCompleted_Throws()
    {
        var request = new PasswordResetRequest(Guid.NewGuid());
        request.Complete(Guid.NewGuid(), DateTime.UtcNow);

        Assert.Throws<InvalidOperationException>(() =>
            request.Complete(Guid.NewGuid(), DateTime.UtcNow));
    }
}

public sealed class RefreshTokenTests
{
    [Fact]
    public void Revoke_WhenCalledTwice_PreservesFirstRevocationTime()
    {
        var token = CreateToken();
        var first = new DateTime(2026, 8, 20, 10, 0, 0, DateTimeKind.Utc);

        token.Revoke(first);
        token.Revoke(first.AddMinutes(5));

        Assert.Equal(first, token.RevokedAtUtc);
    }

    [Fact]
    public void ReplaceWith_RevokesAndLinksReplacement()
    {
        var token = CreateToken();
        var replacementId = Guid.NewGuid();
        var revokedAtUtc = DateTime.UtcNow;

        token.ReplaceWith(replacementId, revokedAtUtc);

        Assert.Equal(replacementId, token.ReplacedByTokenId);
        Assert.Equal(revokedAtUtc, token.RevokedAtUtc);
    }

    [Fact]
    public void Constructor_RejectsBlankHash()
    {
        Assert.Throws<ArgumentException>(() =>
            new RefreshToken(Guid.NewGuid(), " ", DateTime.UtcNow.AddDays(1)));
    }

    private static RefreshToken CreateToken() => new(
        Guid.NewGuid(),
        "C7919F17D32C85C0A3B08AD6F7790737E4689785834C41B98D9C42445A59A2A8",
        DateTime.UtcNow.AddDays(7));
}

public sealed class AppUserTests
{
    [Fact]
    public void SetActive_ChangesAccountStatus()
    {
        var user = new AppUser();

        user.SetActive(false);

        Assert.False(user.IsActive);
    }
}

public sealed class AuditLogTests
{
    [Fact]
    public void Constructor_StoresAuditData()
    {
        var changedBy = Guid.NewGuid();
        var changedAtUtc = new DateTime(2026, 8, 20, 9, 0, 0, DateTimeKind.Utc);
        var auditLog = new AuditLog(
            "employees", "employee-id", "Modified", "[]", null, null,
            changedBy, "admin@example.com", changedAtUtc);

        Assert.Equal("employees", auditLog.TableName);
        Assert.Equal("employee-id", auditLog.RecordId);
        Assert.Equal("Modified", auditLog.Action);
        Assert.Equal(changedBy, auditLog.ChangedBy);
        Assert.Equal(changedAtUtc, auditLog.ChangedAtUtc);
    }

    [Fact]
    public void Constructor_RejectsBlankTableName()
    {
        Assert.Throws<ArgumentException>(() => new AuditLog(
            " ", "record-id", "Added", "[]",
            null, null, null, null, DateTime.UtcNow));
    }
}