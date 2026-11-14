using marketgo_backend.Requisition.Domain.Model.Aggregates;
namespace marketgo_backend.Requisition.Domain.Repositories;
public interface ISupplyRequestRepository
{
    Task AddAsync(SupplyRequest request, CancellationToken cancellationToken);
    Task<SupplyRequest?> FindAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<SupplyRequest>> ListForAsync(Guid organizationId, string accountType, CancellationToken cancellationToken);
}
