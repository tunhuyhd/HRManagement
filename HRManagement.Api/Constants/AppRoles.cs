namespace HRManagement.Api.Constants;

public static class AppRoles
{
    public const string Admin = "ADMIN";
    public const string HrManager = "HR_MANAGER";
    public const string User = "USER";

    public static readonly string[] All = [Admin, HrManager, User];

    public static readonly string[] AssignableByAdmin = [HrManager, User];
}
