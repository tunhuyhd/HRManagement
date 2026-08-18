namespace HRManagement.Api.Common.Auth;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }
    Guid? Id { get; }
    string? Email { get; }
    bool IsInRole(string role);
}
