using marketgo_backend.Shared.Domain.Model.Events;

namespace marketgo_backend.Profiles.Domain.Model.Events;

public sealed record ProfileCreatedEvent(Guid OrganizationId, string Email) : IEvent;
