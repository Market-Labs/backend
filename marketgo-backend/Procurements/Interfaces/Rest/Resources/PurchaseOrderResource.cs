using marketgo_backend.Procurements.Domain.Model.ValueObjects;
namespace marketgo_backend.Procurements.Interfaces.Rest.Resources;
public sealed record PurchaseOrderResource(Guid Id, Guid SupplyRequestId, Guid SupplierId, Guid MinimarketId,
    string Supplier, string Minimarket, string Status, decimal Total, DateTimeOffset? CreatedAt,
    DateOnly ShippingDate, DateTimeOffset? ReceivedAt, string Observations, string RejectionReason,
    IReadOnlyList<PurchaseOrderItem> Items);
