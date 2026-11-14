using marketgo_backend.Shared.Domain.Model.Events;
namespace marketgo_backend.Requisition.Domain.Model.Events;
public sealed record SupplyRequestReviewedEvent(Guid RequestId, Guid SupplierId, string Status) : IEvent;
