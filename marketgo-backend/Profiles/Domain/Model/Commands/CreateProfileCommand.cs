namespace marketgo_backend.Profiles.Domain.Model.Commands;

public sealed record CreateProfileCommand(Guid OrganizationId, string OwnerEmail, string Type,
    string BusinessName, string Email, string? Phone, string? Address, string? District,
    string? Specialty, string? CoverageArea);
