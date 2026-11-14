namespace marketgo_backend.Inventory.Interfaces.Acl;
public sealed record InventoryMetrics(int Stock, int ItemCount, int RiskCount, int WasteQuantity);
public interface IInventoryContextFacade
{
    Task<InventoryMetrics> GetMetricsAsync(Guid organizationId, CancellationToken cancellationToken);
}
