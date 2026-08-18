using System.ComponentModel.DataAnnotations;
using HRManagement.Api.Constants;

namespace HRManagement.Api.Features.Users.Models;

public sealed record UpdateUserAccessRequest(
    [Required] string Role,
    bool IsActive) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!AppRoles.AssignableByAdmin.Contains(Role, StringComparer.OrdinalIgnoreCase))
        {
            yield return new ValidationResult(
                $"Role must be one of: {string.Join(", ", AppRoles.AssignableByAdmin)}.",
                [nameof(Role)]);
        }
    }
}
