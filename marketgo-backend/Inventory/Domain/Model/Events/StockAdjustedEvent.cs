using marketgo_backend.Shared.Domain.Model.Events;
namespace marketgo_backend.Inventory.Domain.Model.Events;
public sealed record StockAdjustedEvent(Guid InventoryItemId, Guid OrganizationId, int NewStock) : IEvent;
