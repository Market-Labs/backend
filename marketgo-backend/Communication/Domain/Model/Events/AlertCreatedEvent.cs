using marketgo_backend.Shared.Domain.Model.Events;
namespace marketgo_backend.Communication.Domain.Model.Events;
public sealed record AlertCreatedEvent(Guid AlertId, Guid OrganizationId) : IEvent;
