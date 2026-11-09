namespace marketgo_backend.Profiles.Application.Services;

public sealed record ProfileResource(Guid Id, Guid UserId, string Type, string BusinessName,
    string Email, string Phone, string Address, string District, string Specialty, string CoverageArea);

public sealed record UpdateProfileCommand(string? BusinessName, string? Email, string? Phone,
    string? Address, string? District, string? Specialty, string? CoverageArea);

public enum ProfileError { NotFound, InvalidInput }
