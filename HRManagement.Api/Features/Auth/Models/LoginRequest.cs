using System.ComponentModel.DataAnnotations;

namespace HRManagement.Api.Features.Auth.Models;

public sealed record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password);
