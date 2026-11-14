using System.Text.Json;
using System.Text.Json.Serialization;
using marketgo_backend.Requisition.Domain.Model.Events;
using marketgo_backend.Requisition.Domain.Model.ValueObjects;
using marketgo_backend.Shared.Domain.Model.Aggregates.Internal;
namespace marketgo_backend.Requisition.Domain.Model.Aggregates;
public partial class SupplyRequest : AggregateRoot
{
    private SupplyRequest() { }
    public SupplyRequest(Guid minimarketId, Guid supplierId, int requesterId, string requester,
        string supplier, string reason, IReadOnlyList<SupplyRequestItem> items)
    {
        if (minimarketId == Guid.Empty || supplierId == Guid.Empty || minimarketId == supplierId ||
            requesterId <= 0 || string.IsNullOrWhiteSpace(requester) || requester.Length > 160 ||
            string.IsNullOrWhiteSpace(supplier) || supplier.Length > 160 ||
            string.IsNullOrWhiteSpace(reason) || reason.Trim().Length > 1000 ||
            items.Count is < 1 or > 100) throw new ArgumentException("Invalid supply request.");
        foreach (var item in items) item.Validate();
        Id = Guid.NewGuid(); MinimarketId = minimarketId; SupplierId = supplierId;
        RequesterId = requesterId; Requester = requester.Trim(); Supplier = supplier.Trim();
        Reason = reason.Trim(); ItemsJson = JsonSerializer.Serialize(items); Status = "pending";
    }
    public Guid Id { get; private set; }
    public Guid MinimarketId { get; private set; }
    public Guid SupplierId { get; private set; }
    public int RequesterId { get; private set; }
    public string Requester { get; private set; } = string.Empty;
    public string Supplier { get; private set; } = string.Empty;
    public string Reason { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;
    public DateTimeOffset? ReviewedAt { get; private set; }
    public string RejectionReason { get; private set; } = string.Empty;
    public Guid? ShippingOrderId { get; private set; }
    [JsonIgnore] public string ItemsJson { get; private set; } = "[]";
    public IReadOnlyList<SupplyRequestItem> Items => JsonSerializer.Deserialize<List<SupplyRequestItem>>(ItemsJson) ?? [];
    public void Review(string status, string? rejectionReason, DateTimeOffset now)
    {
        if (Status != "pending" || status is not ("accepted" or "rejected"))
            throw new InvalidOperationException("Request cannot be reviewed in this state.");
        if (status == "rejected" && (string.IsNullOrWhiteSpace(rejectionReason) || rejectionReason.Length > 1000))
            throw new ArgumentException("Rejection reason is required.");
        Status = status; ReviewedAt = now; RejectionReason = status == "rejected" ? rejectionReason!.Trim() : string.Empty;
        AddDomainEvent(new SupplyRequestReviewedEvent(Id, SupplierId, status));
    }
    public void LinkOrder(Guid orderId)
    {
        if (Status != "accepted" || ShippingOrderId.HasValue || orderId == Guid.Empty)
            throw new InvalidOperationException("Only an accepted request can be linked once.");
        ShippingOrderId = orderId;
    }
}
