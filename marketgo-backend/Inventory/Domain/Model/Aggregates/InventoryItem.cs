using marketgo_backend.Inventory.Domain.Model.Events;
using marketgo_backend.Inventory.Domain.Model.ValueObjects;
using marketgo_backend.Shared.Domain.Model.Aggregates.Internal;

namespace marketgo_backend.Inventory.Domain.Model.Aggregates;

public partial class InventoryItem : AggregateRoot
{
    private InventoryItem() { }
    public InventoryItem(Guid organizationId, Guid productId, string productName, int stock,
        int minimumStock, string lotCode, DateOnly expirationDate)
    {
        if (organizationId == Guid.Empty || productId == Guid.Empty ||
            string.IsNullOrWhiteSpace(productName) || productName.Trim().Length > 160)
            throw new ArgumentException("Invalid inventory reference.");
        Id = Guid.NewGuid();
        OrganizationId = organizationId;
        ProductId = productId;
        ProductName = productName.Trim();
        LotCode = new LotCode(lotCode).Value;
        ExpirationDate = expirationDate;
        Update(stock, minimumStock);
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

    public void Update(int stock, int minimumStock)
    {
        if (stock < 0 || minimumStock < 0) throw new ArgumentOutOfRangeException(nameof(stock));
        Stock = stock;
        MinimumStock = minimumStock;
        if (Id != Guid.Empty) AddDomainEvent(new StockAdjustedEvent(Id, OrganizationId, Stock));
    }
    public void RemoveStock(int quantity)
    {
        if (quantity <= 0 || quantity > Stock)
            throw new ArgumentException("Quantity exceeds available stock.", nameof(quantity));
        Update(Stock - quantity, MinimumStock);
    }
}
