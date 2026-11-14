using marketgo_backend.Shared.Domain.Model.Errors;
namespace marketgo_backend.Requisition.Domain.Model.Errors;
public static class RequisitionErrors
{
    public static readonly Error NotFound = Error.NotFound("Requisition.NotFound", "Request not found.");
    public static readonly Error InvalidData = Error.Validation("Requisition.InvalidData", "Invalid request data.");
    public static readonly Error InvalidTransition = Error.Conflict("Requisition.InvalidTransition", "Request cannot change to this status.");
}
