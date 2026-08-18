using System.ComponentModel.DataAnnotations;

namespace HRManagement.Api.Features.Users.Models;

public sealed class UserListQuery
{
    [Range(1, int.MaxValue)]
    public int PageNumber { get; init; } = 1;

    [Range(1, 100)]
    public int PageSize { get; init; } = 20;

    [MaxLength(256)]
    public string? Search { get; init; }
}
