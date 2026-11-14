using marketgo_backend.Shared.Domain.Model.Events;
namespace marketgo_backend.Procurements.Domain.Model.Events;
public sealed record PurchaseOrderReviewedEvent(Guid OrderId, Guid MinimarketId, string Status) : IEvent;
