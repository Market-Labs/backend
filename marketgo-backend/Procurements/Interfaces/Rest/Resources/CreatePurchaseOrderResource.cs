using marketgo_backend.Procurements.Domain.Model.ValueObjects;
namespace marketgo_backend.Procurements.Interfaces.Rest.Resources;
public sealed record CreatePurchaseOrderResource(Guid SupplyRequestId, string Supplier, string Minimarket,
    DateOnly ShippingDate, string? Observations, IReadOnlyList<PurchaseOrderItem> Items);
