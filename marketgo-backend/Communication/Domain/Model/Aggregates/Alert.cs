using marketgo_backend.Shared.Domain.Model;

namespace marketgo_backend.Communication.Domain.Model.Aggregates;

public sealed class Alert : IAuditableEntity
{
    private Alert() { }
    public Alert(Guid organizationId, string title, string message, string type)
    {
        if (organizationId == Guid.Empty || string.IsNullOrWhiteSpace(title) || title.Length > 200 ||
            string.IsNullOrWhiteSpace(message) || message.Length > 2000 || type is not ("info" or "warning" or "critical"))
            throw new ArgumentException("Invalid alert.");
        Id = Guid.NewGuid(); OrganizationId = organizationId; Title = title.Trim(); Message = message.Trim(); Type = type;
    }
    public Guid Id { get; private set; }
    public Guid OrganizationId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public string Type { get; private set; } = string.Empty;
    public bool Read { get; private set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public void MarkRead() => Read = true;
}
