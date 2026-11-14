using marketgo_backend.Shared.Domain.Model.Events;
namespace marketgo_backend.Products.Domain.Model.Events;
public sealed record ProductCreatedEvent(Guid ProductId, Guid OrganizationId) : IEvent;
