namespace marketgo_backend.Profiles.Interfaces.Rest.Resources;

public sealed record ProfileResource(int Id, int UserId, Guid OrganizationId, string Type,
    string BusinessName, string Email, string Phone, string Address, string District,
    string Specialty, string CoverageArea);
