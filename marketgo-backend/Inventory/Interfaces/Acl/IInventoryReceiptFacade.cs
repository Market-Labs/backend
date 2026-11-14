namespace marketgo_backend.Inventory.Interfaces.Acl;
public sealed record InventoryReceiptOrderItem(string ProductName, int Quantity, decimal UnitPrice);
public sealed record InventoryReceiptLot(string LotCode, DateOnly ExpirationDate, int Quantity);
public sealed record InventoryReceiptLine(int LineIndex, Guid? ProductId, string? Category,
    string? Description, IReadOnlyList<InventoryReceiptLot> Lots);
public interface IInventoryReceiptFacade
{
    Task ReceiveAsync(Guid organizationId, Guid supplierId, Guid purchaseOrderId,
        IReadOnlyList<InventoryReceiptOrderItem> orderItems,
        IReadOnlyList<InventoryReceiptLine> receivedLines, CancellationToken cancellationToken);
}
