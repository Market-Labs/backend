namespace marketgo_backend.Profiles.Interfaces.Rest.Resources;

public sealed record PublicProfileResource(int Id, Guid OrganizationId, string Type,
    string BusinessName, string District, string CoverageArea);
