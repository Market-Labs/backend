namespace marketgo_backend.Suppliers.Domain.Model.ValueObjects;

public readonly record struct RucNumber
{
    public RucNumber(string value)
    {
        if (value is null || value.Length != 11 || !value.All(char.IsAsciiDigit))
            throw new ArgumentException("RUC must contain 11 digits.", nameof(value));
        Value = value;
    }
    public string Value { get; }
    public override string ToString() => Value;
}
