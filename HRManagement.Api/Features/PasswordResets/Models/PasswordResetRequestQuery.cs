using System.ComponentModel.DataAnnotations;
using HRManagement.Api.Constants;

namespace HRManagement.Api.Features.PasswordResets.Models;

public sealed class PasswordResetRequestQuery
{
    [Range(1, int.MaxValue)]
    public int PageNumber { get; init; } = 1;

    [Range(1, 100)]
    public int PageSize { get; init; } = 20;

    [MaxLength(256)]
    public string? Search { get; init; }

    [EnumDataType(typeof(PasswordResetStatus))]
    public PasswordResetStatus? Status { get; init; }
}
