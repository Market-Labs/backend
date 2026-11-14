using marketgo_backend.Requisition.Domain.Model.ValueObjects;
namespace marketgo_backend.Requisition.Interfaces.Rest.Resources;
public sealed record SupplyRequestResource(Guid Id, Guid MinimarketId, Guid SupplierId, int RequesterId,
    string Requester, string Supplier, string ProductName, int Quantity, string Reason,
    string Status, DateTimeOffset? CreatedAt, DateTimeOffset? ReviewedAt, string RejectionReason,
    Guid? ShippingOrderId, IReadOnlyList<SupplyRequestItem> Items);
