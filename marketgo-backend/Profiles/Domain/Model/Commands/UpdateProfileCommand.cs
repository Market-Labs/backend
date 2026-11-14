namespace marketgo_backend.Profiles.Domain.Model.Commands;

public sealed record UpdateProfileCommand(int ProfileId, Guid OrganizationId,
    string? BusinessName, string? Email, string? Phone, string? Address,
    string? District, string? Specialty, string? CoverageArea);
