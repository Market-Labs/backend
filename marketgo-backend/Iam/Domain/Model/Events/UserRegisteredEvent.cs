using marketgo_backend.Shared.Domain.Model.Events;

namespace marketgo_backend.Iam.Domain.Model.Events;

public sealed record UserRegisteredEvent(Guid OrganizationId, string Email, string OrganizationName,
    string AccountType) : IEvent;
