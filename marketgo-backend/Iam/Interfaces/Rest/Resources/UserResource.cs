namespace marketgo_backend.Iam.Interfaces.Rest.Resources;

public sealed record UserResource(
    Guid Id,
    string FullName,
    string Email,
    string Role,
    Guid OrganizationId,
    string OrganizationName,
    string AccountType,
    DateTimeOffset? LastSignedInAt);
