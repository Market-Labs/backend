using marketgo_backend.Communication.Domain.Model.Events;
using marketgo_backend.Shared.Domain.Model.Aggregates.Internal;
namespace marketgo_backend.Communication.Domain.Model.Aggregates;
public partial class Alert : AggregateRoot
{
    private Alert() { }
    public Alert(Guid organizationId, string subject, string body, string severity)
    {
        if (organizationId == Guid.Empty || string.IsNullOrWhiteSpace(subject) ||
            subject.Trim().Length > 200 || string.IsNullOrWhiteSpace(body) ||
            body.Trim().Length > 2000 || severity is not ("info" or "warning" or "critical"))
            throw new ArgumentException("Invalid alert.");
        Id = Guid.NewGuid(); OrganizationId = organizationId; Subject = subject.Trim();
        Body = body.Trim(); Severity = severity; SentAt = DateTimeOffset.UtcNow;
        AddDomainEvent(new AlertCreatedEvent(Id, OrganizationId));
    }
    public Guid Id { get; private set; }
    public Guid OrganizationId { get; private set; }
    public string Sender => "Sistema MarketGo";
    public string Subject { get; private set; } = string.Empty;
    public string Body { get; private set; } = string.Empty;
    public string Severity { get; private set; } = string.Empty;
    public bool Read { get; private set; }
    public bool Starred { get; private set; }
    public DateTimeOffset SentAt { get; private set; }
    public void Update(bool? read, bool? starred) { Read = read ?? Read; Starred = starred ?? Starred; }
}
