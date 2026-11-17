using marketgo_backend.Inventory.Domain.Model.Aggregates;
using marketgo_backend.Inventory.Domain.Repositories;
using marketgo_backend.Inventory.Interfaces.Acl;

namespace marketgo_backend.Inventory.Application.Acl;

public sealed class InventoryCatalogFacade(IInventoryRepository inventory) : IInventoryCatalogFacade
{
    public Task AddInitialLotAsync(Guid organizationId, Guid productId, string productName,
        int quantity, DateOnly expirationDate, CancellationToken cancellationToken) =>
        inventory.AddAsync(new InventoryItem(organizationId, productId, productName, quantity,
            0, $"INIT-{productId:N}", expirationDate), cancellationToken);
}
