namespace marketgo_backend.Iam.Domain.Model.Commands;

public sealed record SignUpCommand(string FullName, string Email, string Password, string OrganizationName,
    string AccountType, bool AcceptedTerms);
