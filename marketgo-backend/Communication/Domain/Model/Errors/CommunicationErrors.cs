using marketgo_backend.Shared.Domain.Model.Errors;
namespace marketgo_backend.Communication.Domain.Model.Errors;
public static class CommunicationErrors
{
    public static readonly Error NotFound = Error.NotFound("Communication.NotFound", "Message or alert not found.");
    public static readonly Error InvalidData = Error.Validation("Communication.InvalidData", "Invalid communication data.");
    public static readonly Error RecipientInvalid = Error.NotFound("Communication.RecipientInvalid", "Recipient organization not found.");
    public static readonly Error NotLinked = Error.Validation("Communication.NotLinked", "Organizations are not linked.");
    public static readonly Error DuplicateKey = Error.Conflict("Communication.DuplicateKey", "Idempotency key was used for another message.");
}
