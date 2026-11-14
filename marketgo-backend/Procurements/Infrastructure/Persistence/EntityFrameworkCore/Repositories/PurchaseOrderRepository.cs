using marketgo_backend.Procurements.Domain.Model.Aggregates;
using marketgo_backend.Procurements.Domain.Repositories;
using marketgo_backend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore;
namespace marketgo_backend.Procurements.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
public sealed class PurchaseOrderRepository(AppDbContext context) : IPurchaseOrderRepository
{
    public async Task AddAsync(PurchaseOrder order, CancellationToken ct) => await context.Set<PurchaseOrder>().AddAsync(order, ct);
    public Task<PurchaseOrder?> FindAsync(Guid id, CancellationToken ct) =>
        context.Set<PurchaseOrder>().FirstOrDefaultAsync(x => x.Id == id, ct);
    public async Task<IReadOnlyList<PurchaseOrder>> ListForAsync(Guid org, string type, CancellationToken ct)
    {
        var query = context.Set<PurchaseOrder>().AsNoTracking();
        query = type == "supplier" ? query.Where(x => x.SupplierId == org) :
            query.Where(x => x.MinimarketId == org);
        return await query.OrderByDescending(x => x.CreatedAt).ToListAsync(ct);
    }
}
