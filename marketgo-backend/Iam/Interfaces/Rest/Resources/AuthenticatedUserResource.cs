namespace marketgo_backend.Iam.Interfaces.Rest.Resources;

public sealed record AuthenticatedUserResource(string Token, UserResource User);
