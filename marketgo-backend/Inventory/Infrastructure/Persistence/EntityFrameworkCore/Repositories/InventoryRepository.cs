using marketgo_backend.Inventory.Domain.Model.Aggregates;
using marketgo_backend.Inventory.Domain.Repositories;
using marketgo_backend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using marketgo_backend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
namespace marketgo_backend.Inventory.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
public sealed class InventoryRepository(AppDbContext context) : OrganizationRepository<InventoryItem>(context), IInventoryRepository
{
    public async Task<IReadOnlyList<InventoryItem>> SearchAsync(Guid organizationId, string? text, DateOnly? expiringBy, CancellationToken ct)
    {
        var query = Context.Set<InventoryItem>().AsNoTracking().Where(x => x.OrganizationId == organizationId);
        if (!string.IsNullOrWhiteSpace(text)) query = query.Where(x => x.ProductName.Contains(text) || x.LotCode.Contains(text));
        if (expiringBy.HasValue) query = query.Where(x => x.ExpirationDate <= expiringBy.Value);
        return await query.OrderBy(x => x.ExpirationDate).ToListAsync(ct);
    }
    public async Task<IReadOnlyList<WasteRecord>> ListWasteAsync(Guid id, CancellationToken ct) =>
        await Context.Set<WasteRecord>().AsNoTracking().Where(x => x.OrganizationId == id)
            .OrderByDescending(x => x.RecordedAt).Take(500).ToListAsync(ct);
    public async Task<IReadOnlyList<DonationRecord>> ListDonationsAsync(Guid id, CancellationToken ct) =>
        await Context.Set<DonationRecord>().AsNoTracking().Where(x => x.OrganizationId == id)
            .OrderByDescending(x => x.RecordedAt).Take(500).ToListAsync(ct);
    public async Task AddWasteAsync(WasteRecord waste, CancellationToken ct) =>
        await Context.Set<WasteRecord>().AddAsync(waste, ct);
    public async Task AddDonationAsync(DonationRecord donation, CancellationToken ct) =>
        await Context.Set<DonationRecord>().AddAsync(donation, ct);
}
