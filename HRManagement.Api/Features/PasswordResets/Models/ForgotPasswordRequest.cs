using System.ComponentModel.DataAnnotations;

namespace HRManagement.Api.Features.PasswordResets.Models;

public sealed record ForgotPasswordRequest(
    [Required, EmailAddress, MaxLength(256)] string Email);
