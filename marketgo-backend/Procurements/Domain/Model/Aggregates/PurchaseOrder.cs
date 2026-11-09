using System.Text.Json;
using System.Text.Json.Serialization;
using marketgo_backend.Shared.Domain.Model;

namespace marketgo_backend.Procurements.Domain.Model.Aggregates;

public sealed record PurchaseOrderItem(string ProductName, int Quantity, decimal UnitPrice);

public sealed class PurchaseOrder : IAuditableEntity
{
    private PurchaseOrder() { }
    public PurchaseOrder(Guid supplierId, Guid minimarketId, Guid? supplyRequestId, DateOnly shippingDate,
        string observations, IReadOnlyList<PurchaseOrderItem> items)
    {
        if (supplierId == Guid.Empty || minimarketId == Guid.Empty) throw new ArgumentException("Parties are required.");
        if (items.Count == 0 || items.Any(x => string.IsNullOrWhiteSpace(x.ProductName) || x.ProductName.Length > 160 || x.Quantity <= 0 || x.UnitPrice <= 0))
            throw new ArgumentException("Invalid order items.");
        if (observations.Length > 1000) throw new ArgumentException("Observations are too long.");
        Id = Guid.NewGuid();
        SupplierId = supplierId;
        MinimarketId = minimarketId;
        SupplyRequestId = supplyRequestId;
        ShippingDate = shippingDate;
        Observations = observations.Trim();
        ItemsJson = JsonSerializer.Serialize(items);
        Total = items.Sum(x => x.Quantity * x.UnitPrice);
        Status = "pending-reception";
    }

    public Guid Id { get; private set; }
    public Guid SupplierId { get; private set; }
    public Guid MinimarketId { get; private set; }
    public Guid? SupplyRequestId { get; private set; }
    public DateOnly ShippingDate { get; private set; }
    public string Observations { get; private set; } = string.Empty;
    public decimal Total { get; private set; }
    public string Status { get; private set; } = string.Empty;
    public DateTimeOffset? ReceivedAt { get; private set; }
    public string RejectionReason { get; private set; } = string.Empty;
    [JsonIgnore] public string ItemsJson { get; private set; } = "[]";
    public IReadOnlyList<PurchaseOrderItem> Items => JsonSerializer.Deserialize<List<PurchaseOrderItem>>(ItemsJson) ?? [];
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public void ReviewReception(bool accepted, string? rejectionReason, DateTimeOffset now)
    {
        if (Status != "pending-reception") throw new InvalidOperationException("Order has already been reviewed.");
        if (!accepted && (string.IsNullOrWhiteSpace(rejectionReason) || rejectionReason.Trim().Length > 1000))
            throw new ArgumentException("Rejection reason must contain 1 to 1000 characters.");
        Status = accepted ? "received" : "rejected";
        ReceivedAt = accepted ? now : null;
        RejectionReason = accepted ? string.Empty : rejectionReason!.Trim();
    }
}
