namespace marketgo_backend.Iam.Interfaces.Rest.Resources;

public sealed record AuthenticatedUserResource(
    string AccessToken,
    string TokenType,
    DateTimeOffset ExpiresAt,
    UserResource User);
