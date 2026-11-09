using System.Text.Json;
using System.Text.Json.Serialization;
using marketgo_backend.Shared.Domain.Model;

namespace marketgo_backend.Requisition.Domain.Model.Aggregates;

public sealed record SupplyRequestItem(string ProductName, int Quantity);

public sealed class SupplyRequest : IAuditableEntity
{
    private SupplyRequest() { }
    public SupplyRequest(Guid minimarketId, Guid supplierId, Guid requesterId, string reason, IReadOnlyList<SupplyRequestItem> items)
    {
        if (minimarketId == Guid.Empty || supplierId == Guid.Empty || requesterId == Guid.Empty) throw new ArgumentException("Parties are required.");
        if (string.IsNullOrWhiteSpace(reason) || reason.Length > 1000 || items.Count == 0 ||
            items.Any(x => string.IsNullOrWhiteSpace(x.ProductName) || x.ProductName.Length > 160 || x.Quantity <= 0))
            throw new ArgumentException("Invalid supply request.");
        Id = Guid.NewGuid();
        MinimarketId = minimarketId;
        SupplierId = supplierId;
        RequesterId = requesterId;
        Reason = reason.Trim();
        ItemsJson = JsonSerializer.Serialize(items);
        Status = "pending";
    }

    public Guid Id { get; private set; }
    public Guid MinimarketId { get; private set; }
    public Guid SupplierId { get; private set; }
    public Guid RequesterId { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;
    public DateTimeOffset? ReviewedAt { get; private set; }
    public string RejectionReason { get; private set; } = string.Empty;
    public Guid? ShippingOrderId { get; private set; }
    [JsonIgnore] public string ItemsJson { get; private set; } = "[]";
    public IReadOnlyList<SupplyRequestItem> Items => JsonSerializer.Deserialize<List<SupplyRequestItem>>(ItemsJson) ?? [];
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public void Review(bool accepted, string? rejectionReason, DateTimeOffset now)
    {
        if (Status != "pending") throw new InvalidOperationException("Request has already been reviewed.");
        if (!accepted && (string.IsNullOrWhiteSpace(rejectionReason) || rejectionReason.Trim().Length > 1000))
            throw new ArgumentException("Rejection reason must contain 1 to 1000 characters.");
        Status = accepted ? "accepted" : "rejected";
        RejectionReason = accepted ? string.Empty : rejectionReason!.Trim();
        ReviewedAt = now;
    }

    public void LinkOrder(Guid orderId)
    {
        if (Status != "accepted" || ShippingOrderId is not null || orderId == Guid.Empty)
            throw new InvalidOperationException("Only an accepted request without an order can be linked.");
        ShippingOrderId = orderId;
    }
}
