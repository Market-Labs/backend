namespace marketgo_backend.Conservation.Domain.Model.Commands;
public sealed record RecordConservationCommand(Guid OrganizationId, string Zone, string ProductName,
    decimal Temperature, decimal Humidity, DateTimeOffset RecordedAt);
