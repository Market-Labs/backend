using marketgo_backend.Shared.Domain.Model;

namespace marketgo_backend.Conservation.Domain.Model.Aggregates;

public sealed class ConservationReading : IAuditableEntity
{
    private ConservationReading() { }
    public ConservationReading(Guid organizationId, string zone, string productName, decimal temperature, decimal humidity, DateTimeOffset recordedAt)
    {
        if (organizationId == Guid.Empty || string.IsNullOrWhiteSpace(zone) || zone.Length > 100 ||
            string.IsNullOrWhiteSpace(productName) || productName.Length > 160 ||
            temperature is < -100 or > 100 || humidity is < 0 or > 100)
            throw new ArgumentException("Invalid conservation reading.");
        Id = Guid.NewGuid(); OrganizationId = organizationId; Zone = zone.Trim(); ProductName = productName.Trim();
        Temperature = temperature; Humidity = humidity; RecordedAt = recordedAt;
    }
    public Guid Id { get; private set; }
    public Guid OrganizationId { get; private set; }
    public string Zone { get; private set; } = string.Empty;
    public string ProductName { get; private set; } = string.Empty;
    public decimal Temperature { get; private set; }
    public decimal Humidity { get; private set; }
    public DateTimeOffset RecordedAt { get; private set; }
    public string Status => Temperature is < 0 or > 8 || Humidity is < 30 or > 80 ? "risk" : "healthy";
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
