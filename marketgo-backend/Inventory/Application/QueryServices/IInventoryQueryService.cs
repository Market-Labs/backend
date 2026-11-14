using marketgo_backend.Inventory.Domain.Model.Aggregates;
using marketgo_backend.Inventory.Domain.Model.Queries;
namespace marketgo_backend.Inventory.Application.QueryServices;
public interface IInventoryQueryService
{
    Task<IReadOnlyList<InventoryItem>> Handle(GetInventoryQuery query, CancellationToken cancellationToken);
    Task<InventoryItem?> FindAsync(Guid id, Guid organizationId, CancellationToken cancellationToken);
    Task<IReadOnlyList<WasteRecord>> ListWasteAsync(Guid organizationId, CancellationToken cancellationToken);
    Task<IReadOnlyList<DonationRecord>> ListDonationsAsync(Guid organizationId, CancellationToken cancellationToken);
}
