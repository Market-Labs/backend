using System.Text.Json;
using System.Text.Json.Serialization;
using marketgo_backend.Procurements.Domain.Model.Events;
using marketgo_backend.Procurements.Domain.Model.ValueObjects;
using marketgo_backend.Shared.Domain.Model.Aggregates.Internal;
namespace marketgo_backend.Procurements.Domain.Model.Aggregates;
public partial class PurchaseOrder : AggregateRoot
{
    private PurchaseOrder() { }
    public PurchaseOrder(Guid supplierId, Guid minimarketId, Guid supplyRequestId, string supplier,
        string minimarket, DateOnly shippingDate, string? observations, IReadOnlyList<PurchaseOrderItem> items)
    {
        if (supplierId == Guid.Empty || minimarketId == Guid.Empty || supplyRequestId == Guid.Empty ||
            supplierId == minimarketId || string.IsNullOrWhiteSpace(supplier) || supplier.Length > 160 ||
            string.IsNullOrWhiteSpace(minimarket) || minimarket.Length > 160 ||
            observations?.Length > 1000 || items.Count is < 1 or > 100)
            throw new ArgumentException("Invalid purchase order.");
        foreach (var item in items) item.Validate();
        Id = Guid.NewGuid(); SupplierId = supplierId; MinimarketId = minimarketId;
        SupplyRequestId = supplyRequestId; Supplier = supplier.Trim(); Minimarket = minimarket.Trim();
        ShippingDate = shippingDate; Observations = observations?.Trim() ?? string.Empty;
        Total = items.Sum(item => item.Quantity * item.UnitPrice);
        ItemsJson = JsonSerializer.Serialize(items); Status = "pending-reception";
    }
    public Guid Id { get; private set; }
    public Guid SupplierId { get; private set; }
    public Guid MinimarketId { get; private set; }
    public Guid SupplyRequestId { get; private set; }
    public string Supplier { get; private set; } = string.Empty;
    public string Minimarket { get; private set; } = string.Empty;
    public DateOnly ShippingDate { get; private set; }
    public string Observations { get; private set; } = string.Empty;
    public decimal Total { get; private set; }
    public string Status { get; private set; } = string.Empty;
    public DateTimeOffset? ReceivedAt { get; private set; }
    public DateTimeOffset? ReviewedAt { get; private set; }
    public int? ReviewedByUserId { get; private set; }
    public string RejectionReason { get; private set; } = string.Empty;
    [JsonIgnore] public string ItemsJson { get; private set; } = "[]";
    public IReadOnlyList<PurchaseOrderItem> Items => JsonSerializer.Deserialize<List<PurchaseOrderItem>>(ItemsJson) ?? [];
    public void ReviewReception(string status, string? reason, int reviewerId, DateTimeOffset now)
    {
        if (Status != "pending-reception" || status is not ("received" or "rejected"))
            throw new InvalidOperationException("Order cannot be reviewed in this state.");
        if (reviewerId <= 0 || (status == "rejected" && (string.IsNullOrWhiteSpace(reason) || reason.Length > 1000)))
            throw new ArgumentException("A reviewer and rejection reason are required.");
        Status = status; ReviewedByUserId = reviewerId; ReviewedAt = now;
        ReceivedAt = status == "received" ? now : null;
        RejectionReason = status == "rejected" ? reason!.Trim() : string.Empty;
        AddDomainEvent(new PurchaseOrderReviewedEvent(Id, MinimarketId, Status));
    }
}
