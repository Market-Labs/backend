namespace marketgo_backend.Conservation.Domain.Model.ValueObjects;
public static class ConservationRange
{
    public static bool IsSafe(decimal temperature, decimal humidity) =>
        temperature is >= 0 and <= 8 && humidity is >= 30 and <= 80;
}
