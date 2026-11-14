namespace marketgo_backend.Inventory.Domain.Model.ValueObjects;
public readonly record struct LotCode
{
    public LotCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Trim().Length > 80)
            throw new ArgumentException("Lot code must contain 1 to 80 characters.", nameof(value));
        Value = value.Trim();
    }
    public string Value { get; }
    public override string ToString() => Value;
}
