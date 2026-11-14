using marketgo_backend.Procurements.Domain.Repositories;
using marketgo_backend.Procurements.Interfaces.Acl;
namespace marketgo_backend.Procurements.Application.Acl;
public sealed class ProcurementsContextFacade(IPurchaseOrderRepository repository) : IProcurementsContextFacade
{
    public async Task<ProcurementMetrics> GetMetricsAsync(Guid id, string type, CancellationToken ct)
    {
        var orders = await repository.ListForAsync(id, type, ct);
        return new(orders.Count, orders.Count(x => x.Status == "pending-reception"));
    }
}
