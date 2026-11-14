using marketgo_backend.Shared.Domain.Model.Errors;
namespace marketgo_backend.Communication.Domain.Model.Errors;
public static class CommunicationErrors
{
    public static readonly Error NotFound = Error.NotFound("Communication.NotFound", "Message or alert not found.");
    public static readonly Error InvalidData = Error.Validation("Communication.InvalidData", "Invalid communication data.");
}
