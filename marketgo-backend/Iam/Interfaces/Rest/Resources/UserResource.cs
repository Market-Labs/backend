namespace marketgo_backend.Iam.Interfaces.Rest.Resources;

public sealed record UserResource(
    int Id, string Name, string Email, string Status, string[] Roles,
    string[] Permissions, Guid? SupplierId, Guid? MinimarketId, Guid TenantId,
    string AccessLevel, Guid OrganizationId, string OrganizationName, string AccountType);
