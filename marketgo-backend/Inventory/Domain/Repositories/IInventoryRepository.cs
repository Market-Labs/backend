using marketgo_backend.Inventory.Domain.Model.Aggregates;
using marketgo_backend.Shared.Domain.Repositories;
namespace marketgo_backend.Inventory.Domain.Repositories;
public interface IInventoryRepository : IOrganizationRepository<InventoryItem>
{
    Task<IReadOnlyList<InventoryItem>> SearchAsync(Guid organizationId, string? text, DateOnly? expiringBy, CancellationToken cancellationToken);
    Task<IReadOnlyList<WasteRecord>> ListWasteAsync(Guid organizationId, CancellationToken cancellationToken);
    Task<IReadOnlyList<DonationRecord>> ListDonationsAsync(Guid organizationId, CancellationToken cancellationToken);
    Task AddWasteAsync(WasteRecord waste, CancellationToken cancellationToken);
    Task AddDonationAsync(DonationRecord donation, CancellationToken cancellationToken);
}
