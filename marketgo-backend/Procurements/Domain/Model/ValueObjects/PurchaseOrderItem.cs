namespace marketgo_backend.Procurements.Domain.Model.ValueObjects;
public sealed record PurchaseOrderItem(string ProductName, int Quantity, decimal UnitPrice)
{
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ProductName) || ProductName.Trim().Length > 160 ||
            Quantity <= 0 || UnitPrice is <= 0 or > 9999999999.99m)
            throw new ArgumentException("Invalid order item.");
    }
}
