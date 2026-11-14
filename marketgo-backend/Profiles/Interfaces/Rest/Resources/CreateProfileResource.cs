namespace marketgo_backend.Profiles.Interfaces.Rest.Resources;

public sealed record CreateProfileResource(string BusinessName, string Email, string? Phone,
    string? Address, string? District, string? Specialty, string? CoverageArea);
