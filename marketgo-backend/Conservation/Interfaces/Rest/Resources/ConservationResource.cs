namespace marketgo_backend.Conservation.Interfaces.Rest.Resources;
public sealed record ConservationResource(Guid Id, string Zone, string ProductName,
    decimal Temperature, decimal Humidity, DateTimeOffset RecordedAt, string Status);
