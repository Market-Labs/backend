using marketgo_backend.Inventory.Application.QueryServices;
using marketgo_backend.Inventory.Domain.Model.Aggregates;
using marketgo_backend.Inventory.Domain.Model.Queries;
using marketgo_backend.Inventory.Domain.Repositories;
namespace marketgo_backend.Inventory.Application.Internal.QueryServices;
public sealed class InventoryQueryService(IInventoryRepository inventory) : IInventoryQueryService
{
    public Task<IReadOnlyList<InventoryItem>> Handle(GetInventoryQuery query, CancellationToken ct) =>
        inventory.SearchAsync(query.OrganizationId, query.Search,
            query.ExpiringWithinDays is int days ? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(days)) : null, ct);
    public Task<InventoryItem?> FindAsync(Guid id, Guid organizationId, CancellationToken ct) =>
        inventory.FindByIdForOrganizationAsync(id, organizationId, ct);
    public Task<IReadOnlyList<WasteRecord>> ListWasteAsync(Guid organizationId, CancellationToken ct) =>
        inventory.ListWasteAsync(organizationId, ct);
    public Task<IReadOnlyList<DonationRecord>> ListDonationsAsync(Guid organizationId, CancellationToken ct) =>
        inventory.ListDonationsAsync(organizationId, ct);
}
