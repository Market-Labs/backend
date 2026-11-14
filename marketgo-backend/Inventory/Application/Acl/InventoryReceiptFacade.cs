using marketgo_backend.Inventory.Domain.Model.Aggregates;
using marketgo_backend.Inventory.Domain.Repositories;
using marketgo_backend.Inventory.Interfaces.Acl;
using marketgo_backend.Products.Interfaces.Acl;
namespace marketgo_backend.Inventory.Application.Acl;
public sealed class InventoryReceiptFacade(IInventoryRepository inventory, IProductsContextFacade products)
    : IInventoryReceiptFacade
{
    public async Task ReceiveAsync(Guid organizationId, Guid supplierId, Guid purchaseOrderId,
        IReadOnlyList<InventoryReceiptOrderItem> orderItems, IReadOnlyList<InventoryReceiptLine> receivedLines, CancellationToken ct)
    {
        if (receivedLines.Count != orderItems.Count ||
            receivedLines.Any(x => x is null || x.Lots is null || x.Lots.Count == 0 ||
                x.Lots.Any(lot => lot is null || string.IsNullOrWhiteSpace(lot.LotCode))) ||
            receivedLines.Select(x => x.LineIndex).Distinct().Count() != orderItems.Count ||
            receivedLines.SelectMany(x => x.Lots.Select(lot => lot.LotCode.Trim()))
                .Distinct(StringComparer.OrdinalIgnoreCase).Count() != receivedLines.Sum(x => x.Lots.Count))
            throw new ArgumentException("Every order line needs unique, explicit lot details.");
        foreach (var line in receivedLines)
        {
            if (line.LineIndex < 0 || line.LineIndex >= orderItems.Count || line.Lots.Any(x =>
                    string.IsNullOrWhiteSpace(x.LotCode) || x.LotCode.Trim().Length > 80 ||
                    x.ExpirationDate == default || x.Quantity <= 0) ||
                line.Lots.Sum(x => (long)x.Quantity) != orderItems[line.LineIndex].Quantity)
                throw new ArgumentException("Receipt lot quantities and expiry must match the order.");
        }
        foreach (var line in receivedLines.OrderBy(x => x.LineIndex))
        {
            var ordered = orderItems[line.LineIndex];
            var firstExistingLot = await inventory.FindByLotAsync(organizationId, line.Lots[0].LotCode.Trim(), ct);
            if (firstExistingLot is not null && line.ProductId.HasValue && line.ProductId != firstExistingLot.ProductId)
                throw new ArgumentException("Selected product does not match the existing lot.");
            var product = await products.ResolveForReceiptAsync(organizationId, supplierId,
                ordered.ProductName, line.ProductId ?? firstExistingLot?.ProductId,
                line.Category, line.Description, line.Lots[0].ExpirationDate, ordered.UnitPrice, ct);
            foreach (var lot in line.Lots)
            {
                var existing = await inventory.FindByLotAsync(organizationId, lot.LotCode.Trim(), ct);
                InventoryItem item;
                if (existing is null)
                {
                    item = new InventoryItem(organizationId, product.Id, product.Name, lot.Quantity,
                        0, lot.LotCode, lot.ExpirationDate);
                    await inventory.AddAsync(item, ct);
                }
                else
                {
                    if (existing.ProductId != product.Id || existing.ExpirationDate != lot.ExpirationDate)
                        throw new ArgumentException("Lot already belongs to another product or expiry.");
                    existing.ReceiveStock(lot.Quantity);
                    item = existing;
                }
                await inventory.AddReceiptMovementAsync(new InventoryReceiptMovement(organizationId,
                    purchaseOrderId, line.LineIndex, item.Id, product.Id, lot.LotCode,
                    lot.ExpirationDate, lot.Quantity), ct);
            }
        }
    }
}
