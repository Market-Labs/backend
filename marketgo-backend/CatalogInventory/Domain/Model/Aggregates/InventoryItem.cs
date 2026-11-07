using marketgo_backend.Shared.Domain.Model;

namespace marketgo_backend.CatalogInventory.Domain.Model.Aggregates;

public sealed class InventoryItem : IAuditableEntity
{
    private InventoryItem() { }

    public InventoryItem(Guid organizationId, Guid productId, string productName, int stock,
        int minimumStock, string lotCode, DateOnly expirationDate)
    {
        if (organizationId == Guid.Empty || productId == Guid.Empty) throw new ArgumentException("Organization and product are required.");
        if (string.IsNullOrWhiteSpace(productName) || productName.Trim().Length > 160) throw new ArgumentException("Invalid product name.");
        if (string.IsNullOrWhiteSpace(lotCode) || lotCode.Trim().Length > 80) throw new ArgumentException("Invalid lot code.");
        if (stock < 0 || minimumStock < 0) throw new ArgumentOutOfRangeException(nameof(stock));
        Id = Guid.NewGuid();
        OrganizationId = organizationId;
        ProductId = productId;
        ProductName = productName.Trim();
        Stock = stock;
        MinimumStock = minimumStock;
        LotCode = lotCode.Trim();
        ExpirationDate = expirationDate;
    }

    public Guid Id { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public int Stock { get; private set; }
    public int MinimumStock { get; private set; }
    public string LotCode { get; private set; } = string.Empty;
    public DateOnly ExpirationDate { get; private set; }
    public string Status => Stock <= MinimumStock ? "risk" : "healthy";
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public void Update(int stock, int minimumStock)
    {
        if (stock < 0 || minimumStock < 0) throw new ArgumentOutOfRangeException(nameof(stock));
        Stock = stock;
        MinimumStock = minimumStock;
    }
}
