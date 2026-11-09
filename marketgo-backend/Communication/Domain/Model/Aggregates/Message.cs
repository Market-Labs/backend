using marketgo_backend.Shared.Domain.Model;

namespace marketgo_backend.Communication.Domain.Model.Aggregates;

public sealed class Message : IAuditableEntity
{
    private Message() { }
    public Message(Guid organizationId, string sender, string receiver, string subject, string body)
    {
        if (organizationId == Guid.Empty || string.IsNullOrWhiteSpace(sender) || sender.Length > 160 ||
            string.IsNullOrWhiteSpace(receiver) || receiver.Length > 160 || string.IsNullOrWhiteSpace(subject) ||
            subject.Length > 200 || string.IsNullOrWhiteSpace(body) || body.Length > 4000)
            throw new ArgumentException("Invalid message.");
        Id = Guid.NewGuid(); OrganizationId = organizationId; Sender = sender.Trim(); Receiver = receiver.Trim();
        Subject = subject.Trim(); Body = body.Trim(); SentAt = DateTimeOffset.UtcNow;
    }
    public Guid Id { get; private set; }
    public Guid OrganizationId { get; private set; }
    public string Sender { get; private set; } = string.Empty;
    public string Receiver { get; private set; } = string.Empty;
    public string Subject { get; private set; } = string.Empty;
    public string Body { get; private set; } = string.Empty;
    public bool Read { get; private set; }
    public bool Starred { get; private set; }
    public DateTimeOffset SentAt { get; private set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public void Update(bool? read, bool? starred) { Read = read ?? Read; Starred = starred ?? Starred; }
}
