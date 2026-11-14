using marketgo_backend.Requisition.Domain.Model.Aggregates;
using marketgo_backend.Requisition.Domain.Repositories;
using marketgo_backend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore;
namespace marketgo_backend.Requisition.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
public sealed class SupplyRequestRepository(AppDbContext context) : ISupplyRequestRepository
{
    public async Task AddAsync(SupplyRequest request, CancellationToken ct) => await context.Set<SupplyRequest>().AddAsync(request, ct);
    public Task<SupplyRequest?> FindAsync(Guid id, CancellationToken ct) =>
        context.Set<SupplyRequest>().FirstOrDefaultAsync(x => x.Id == id, ct);
    public async Task<IReadOnlyList<SupplyRequest>> ListForAsync(Guid org, string type, CancellationToken ct)
    {
        var query = context.Set<SupplyRequest>().AsNoTracking();
        query = type == "supplier" ? query.Where(x => x.SupplierId == org) :
            query.Where(x => x.MinimarketId == org);
        return await query.OrderByDescending(x => x.CreatedAt).ToListAsync(ct);
    }
}
