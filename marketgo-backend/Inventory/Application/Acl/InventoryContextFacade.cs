using marketgo_backend.Inventory.Domain.Repositories;
using marketgo_backend.Inventory.Interfaces.Acl;
namespace marketgo_backend.Inventory.Application.Acl;
public sealed class InventoryContextFacade(IInventoryRepository repository) : IInventoryContextFacade
{
    public async Task<InventoryMetrics> GetMetricsAsync(Guid id, CancellationToken ct)
    {
        var items = await repository.ListByOrganizationAsync(id, ct);
        var waste = await repository.ListWasteAsync(id, ct);
        return new(items.Sum(x => x.Stock), items.Count,
            items.Count(x => x.Stock <= x.MinimumStock), waste.Sum(x => x.Quantity));
    }
}
