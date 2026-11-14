using marketgo_backend.Communication.Domain.Model.Aggregates;
using marketgo_backend.Communication.Domain.Repositories;
using marketgo_backend.Conservation.Domain.Model.Events;
using marketgo_backend.Shared.Application.Internal.EventHandlers;
namespace marketgo_backend.Communication.Application.Internal.EventHandlers;
public sealed class ConservationRiskDetectedEventHandler(ICommunicationRepository repository)
    : IEventHandler<ConservationRiskDetectedEvent>
{
    public async Task Handle(ConservationRiskDetectedEvent domainEvent, CancellationToken ct)
    {
        var subject = $"Riesgo de conservación en {domainEvent.Zone}";
        var body = $"{domainEvent.ProductName}: {domainEvent.Temperature} °C y {domainEvent.Humidity}% de humedad.";
        await repository.AddAlertAsync(new Alert(domainEvent.OrganizationId, subject, body, "warning"), ct);
    }
}
