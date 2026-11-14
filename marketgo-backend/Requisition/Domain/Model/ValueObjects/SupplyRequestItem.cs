namespace marketgo_backend.Requisition.Domain.Model.ValueObjects;
public sealed record SupplyRequestItem(string ProductName, int Quantity)
{
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ProductName) || ProductName.Trim().Length > 160 || Quantity <= 0)
            throw new ArgumentException("Invalid requested item.");
    }
}
