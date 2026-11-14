using marketgo_backend.Shared.Domain.Model.Events;
namespace marketgo_backend.Conservation.Domain.Model.Events;
public sealed record ConservationRiskDetectedEvent(Guid OrganizationId, string Zone, string ProductName,
    decimal Temperature, decimal Humidity) : IEvent;
