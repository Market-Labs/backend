namespace marketgo_backend.Conservation.Interfaces.Rest.Resources;
public sealed record RecordConservationResource(string Zone, string ProductName,
    decimal Temperature, decimal Humidity, DateTimeOffset RecordedAt);
