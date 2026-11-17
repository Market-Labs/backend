namespace marketgo_backend.Inventory.Interfaces.Acl;

public interface IInventoryCatalogFacade
{
    Task AddInitialLotAsync(Guid organizationId, Guid productId, string productName,
        int quantity, DateOnly expirationDate, CancellationToken cancellationToken);
}
