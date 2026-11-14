using marketgo_backend.Procurements.Domain.Model.ValueObjects;
namespace marketgo_backend.Procurements.Domain.Model.Commands;
public sealed record CreatePurchaseOrderCommand(Guid SupplierId, Guid SupplyRequestId, string Supplier,
    string Minimarket, DateOnly ShippingDate, string? Observations, IReadOnlyList<PurchaseOrderItem> Items);
