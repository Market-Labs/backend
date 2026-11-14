using marketgo_backend.Conservation.Domain.Model.Events;
using marketgo_backend.Conservation.Domain.Model.ValueObjects;
using marketgo_backend.Shared.Domain.Model.Aggregates.Internal;
namespace marketgo_backend.Conservation.Domain.Model.Aggregates;
public partial class ConservationReading : AggregateRoot
{
    private ConservationReading() { }
    public ConservationReading(Guid organizationId, string zone, string productName,
        decimal temperature, decimal humidity, DateTimeOffset recordedAt)
    {
        if (organizationId == Guid.Empty || string.IsNullOrWhiteSpace(zone) || zone.Trim().Length > 100 ||
            string.IsNullOrWhiteSpace(productName) || productName.Trim().Length > 160 ||
            temperature is < -100 or > 100 || humidity is < 0 or > 100 ||
            recordedAt > DateTimeOffset.UtcNow.AddMinutes(5))
            throw new ArgumentException("Invalid conservation reading.");
        Id = Guid.NewGuid(); OrganizationId = organizationId; Zone = zone.Trim();
        ProductName = productName.Trim(); Temperature = temperature; Humidity = humidity;
        RecordedAt = recordedAt;
        if (Status == "risk") AddDomainEvent(new ConservationRiskDetectedEvent(
            OrganizationId, Zone, ProductName, Temperature, Humidity));
    }
    public Guid Id { get; private set; }
    public Guid OrganizationId { get; private set; }
    public string Zone { get; private set; } = string.Empty;
    public string ProductName { get; private set; } = string.Empty;
    public decimal Temperature { get; private set; }
    public decimal Humidity { get; private set; }
    public DateTimeOffset RecordedAt { get; private set; }
    public string Status => ConservationRange.IsSafe(Temperature, Humidity) ? "healthy" : "risk";
}
