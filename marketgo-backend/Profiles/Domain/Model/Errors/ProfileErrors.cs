using marketgo_backend.Shared.Domain.Model.Errors;

namespace marketgo_backend.Profiles.Domain.Model.Errors;

public static class ProfileErrors
{
    public static readonly Error ProfileNotFound = Error.NotFound("Profiles.ProfileNotFound", "Profile not found.");
    public static readonly Error ProfileAlreadyExists = Error.Conflict("Profiles.ProfileAlreadyExists", "This organization already has a profile.");
    public static readonly Error InvalidProfileData = Error.Validation("Profiles.InvalidProfileData", "Invalid profile data.");
    public static readonly Error Forbidden = Error.Failure("Profiles.Forbidden", "This profile belongs to another organization.");
}
