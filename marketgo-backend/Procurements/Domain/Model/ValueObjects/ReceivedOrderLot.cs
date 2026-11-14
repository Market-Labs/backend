namespace marketgo_backend.Procurements.Domain.Model.ValueObjects;
public sealed record ReceivedOrderLot(string LotCode, DateOnly ExpirationDate, int Quantity);
public sealed record ReceivedOrderLine(int LineIndex, Guid? ProductId, string? Category,
    string? Description, IReadOnlyList<ReceivedOrderLot> Lots);
