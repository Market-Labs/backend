using marketgo_backend.Conservation.Domain.Model.Aggregates;
using marketgo_backend.Conservation.Interfaces.Rest.Resources;
namespace marketgo_backend.Conservation.Interfaces.Rest.Transform;
public static class ConservationResourceFromEntityAssembler
{
    public static ConservationResource ToResourceFromEntity(ConservationReading reading) =>
        new(reading.Id, reading.Zone, reading.ProductName, reading.Temperature,
            reading.Humidity, reading.RecordedAt, reading.Status);
}
