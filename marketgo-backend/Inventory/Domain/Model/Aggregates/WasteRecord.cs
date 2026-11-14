using marketgo_backend.Shared.Domain.Model.Entities;
namespace marketgo_backend.Inventory.Domain.Model.Aggregates;
public sealed class WasteRecord : IAuditableEntity
{
    private WasteRecord() { }
    public WasteRecord(Guid organizationId, InventoryItem item, int quantity, string reason)
    {
        if (organizationId == Guid.Empty || item.OrganizationId != organizationId ||
            string.IsNullOrWhiteSpace(reason) || reason.Trim().Length > 500)
            throw new ArgumentException("Invalid waste record.");
        item.RemoveStock(quantity);
        Id = Guid.NewGuid();
        OrganizationId = organizationId;
        InventoryItemId = item.Id;
        ProductName = item.ProductName;
        LotCode = item.LotCode;
        Quantity = quantity;
        Reason = reason.Trim();
        RecordedAt = DateTimeOffset.UtcNow;
    }
    public Guid Id { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid InventoryItemId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public string LotCode { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public string Unit => "units";
    public string Reason { get; private set; } = string.Empty;
    public DateTimeOffset RecordedAt { get; private set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
