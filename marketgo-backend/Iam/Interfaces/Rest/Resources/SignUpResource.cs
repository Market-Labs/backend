using System.ComponentModel.DataAnnotations;

namespace marketgo_backend.Iam.Interfaces.Rest.Resources;

public sealed class SignUpResource
{
    [Required, StringLength(160, MinimumLength = 1)]
    public string FullName { get; init; } = string.Empty;

    [Required, EmailAddress, StringLength(254)]
    public string Email { get; init; } = string.Empty;

    [Required, StringLength(128, MinimumLength = 8)]
    public string Password { get; init; } = string.Empty;

    [Required, StringLength(160, MinimumLength = 1)]
    public string OrganizationName { get; init; } = string.Empty;

    [Required]
    public string AccountType { get; init; } = string.Empty;

    public bool AcceptedTerms { get; init; }
}
