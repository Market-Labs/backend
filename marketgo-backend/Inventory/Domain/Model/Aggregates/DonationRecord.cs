using marketgo_backend.Shared.Domain.Model;

namespace marketgo_backend.Inventory.Domain.Model.Aggregates;

public sealed class DonationRecord : IAuditableEntity
{
    private DonationRecord() { }
    public DonationRecord(Guid organizationId, InventoryItem item, int quantity, string recipient)
    {
        if (organizationId == Guid.Empty || item.OrganizationId != organizationId || quantity <= 0 || quantity > item.Stock ||
            string.IsNullOrWhiteSpace(recipient) || recipient.Length > 160) throw new ArgumentException("Invalid donation.");
        Id = Guid.NewGuid(); OrganizationId = organizationId; InventoryItemId = item.Id;
        ProductName = item.ProductName; LotCode = item.LotCode; Quantity = quantity;
        Recipient = recipient.Trim(); RecordedAt = DateTimeOffset.UtcNow;
        item.Update(item.Stock - quantity, item.MinimumStock);
    }
    public Guid Id { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid InventoryItemId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public string LotCode { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public string Recipient { get; private set; } = string.Empty;
    public DateTimeOffset RecordedAt { get; private set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
