using System.ComponentModel.DataAnnotations;

namespace HRManagement.Api.Features.PasswordResets.Models;

public sealed record ChangePasswordRequest(
    [Required] string CurrentPassword,
    [Required, MinLength(6)] string NewPassword);
