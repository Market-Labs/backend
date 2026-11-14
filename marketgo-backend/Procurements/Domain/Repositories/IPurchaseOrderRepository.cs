using marketgo_backend.Procurements.Domain.Model.Aggregates;
namespace marketgo_backend.Procurements.Domain.Repositories;
public interface IPurchaseOrderRepository
{
    Task AddAsync(PurchaseOrder order, CancellationToken cancellationToken);
    Task<PurchaseOrder?> FindAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<PurchaseOrder>> ListForAsync(Guid organizationId, string accountType, CancellationToken cancellationToken);
}
