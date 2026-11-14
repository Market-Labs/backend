namespace marketgo_backend.Analytics.Domain.Model.ValueObjects;
public sealed record OperationalIndicator(string Label, decimal CurrentValue, decimal PreviousValue, string Unit);
