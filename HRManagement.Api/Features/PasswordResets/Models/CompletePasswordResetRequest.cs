using System.ComponentModel.DataAnnotations;

namespace HRManagement.Api.Features.PasswordResets.Models;

public sealed record CompletePasswordResetRequest(
    [Required, MinLength(6)] string NewPassword);
