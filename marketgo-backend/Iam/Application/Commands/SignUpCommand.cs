using marketgo_backend.Profiles.Domain.Model.ValueObjects;

namespace marketgo_backend.Iam.Application.Commands;

public sealed record SignUpCommand(
    string FullName,
    string Email,
    string Password,
    string OrganizationName,
    OrganizationKind OrganizationKind,
    bool AcceptedTerms);
