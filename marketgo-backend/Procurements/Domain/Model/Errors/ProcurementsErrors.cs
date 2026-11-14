using marketgo_backend.Shared.Domain.Model.Errors;
namespace marketgo_backend.Procurements.Domain.Model.Errors;
public static class ProcurementsErrors
{
    public static readonly Error NotFound = Error.NotFound("Procurements.NotFound", "Order not found.");
    public static readonly Error InvalidRequest = Error.Conflict("Procurements.InvalidRequest", "A matching accepted request is required.");
    public static readonly Error InvalidData = Error.Validation("Procurements.InvalidData", "Invalid order data.");
    public static readonly Error InvalidTransition = Error.Conflict("Procurements.InvalidTransition", "Order cannot change to this status.");
}
