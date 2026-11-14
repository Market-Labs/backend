using marketgo_backend.Shared.Domain.Model.Errors;

namespace marketgo_backend.Iam.Domain.Model.Errors;

public static class IamErrors
{
    public static readonly Error InvalidCredentials = Error.Unauthorized("Iam.InvalidCredentials", "Invalid email or password.");
    public static readonly Error EmailAlreadyTaken = Error.Conflict("Iam.EmailAlreadyTaken", "Email is already registered.");
    public static readonly Error InvalidRegistration = Error.Validation("Iam.InvalidRegistration", "Invalid registration data.");
    public static readonly Error UserNotFound = Error.NotFound("Iam.UserNotFound", "User was not found.");
}
