using marketgo_backend.Shared.Domain.Model.Errors;
namespace marketgo_backend.Conservation.Domain.Model.Errors;
public static class ConservationErrors
{
    public static readonly Error InvalidReading = Error.Validation("Conservation.InvalidReading", "Invalid conservation reading.");
}
