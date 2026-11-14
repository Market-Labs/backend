namespace marketgo_backend.Iam.Interfaces.Rest.Resources;

public sealed record SignUpResource(
    string FullName, string Email, string Password, string OrganizationName,
    string AccountType, bool AcceptedTerms);
