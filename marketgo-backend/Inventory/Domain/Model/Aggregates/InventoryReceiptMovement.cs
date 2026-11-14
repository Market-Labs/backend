using marketgo_backend.Shared.Domain.Model.Aggregates.Internal;
using marketgo_backend.Shared.Domain.Model.Entities;
namespace marketgo_backend.Inventory.Domain.Model.Aggregates;
public sealed class InventoryReceiptMovement : AggregateRoot, IAuditableEntity
{
    private InventoryReceiptMovement() { }
    public InventoryReceiptMovement(Guid organizationId, Guid purchaseOrderId, int lineIndex,
        Guid inventoryItemId, Guid productId, string lotCode, DateOnly expirationDate, int quantity)
    {
        if (organizationId == Guid.Empty || purchaseOrderId == Guid.Empty || inventoryItemId == Guid.Empty ||
            productId == Guid.Empty || lineIndex < 0 || quantity <= 0 || string.IsNullOrWhiteSpace(lotCode))
            throw new ArgumentException("Invalid receipt movement.");
        Id = Guid.NewGuid(); OrganizationId = organizationId; PurchaseOrderId = purchaseOrderId;
        LineIndex = lineIndex; InventoryItemId = inventoryItemId; ProductId = productId;
        LotCode = lotCode.Trim(); ExpirationDate = expirationDate; Quantity = quantity;
    }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public Guid Id { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid PurchaseOrderId { get; private set; }
    public int LineIndex { get; private set; }
    public Guid InventoryItemId { get; private set; }
    public Guid ProductId { get; private set; }
    public string LotCode { get; private set; } = string.Empty;
    public DateOnly ExpirationDate { get; private set; }
    public int Quantity { get; private set; }
}
