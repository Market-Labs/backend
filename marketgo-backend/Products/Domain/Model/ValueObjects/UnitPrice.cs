namespace marketgo_backend.Products.Domain.Model.ValueObjects;
public readonly record struct UnitPrice
{
    public UnitPrice(decimal value)
    {
        if (value is <= 0 or > 9999999999.99m) throw new ArgumentOutOfRangeException(nameof(value));
        Value = decimal.Round(value, 2);
    }
    public decimal Value { get; }
}
