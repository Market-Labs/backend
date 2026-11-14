namespace marketgo_backend.Procurements.Interfaces.Acl;
public sealed record ProcurementMetrics(int TotalOrders, int PendingReception);
public interface IProcurementsContextFacade
{
    Task<ProcurementMetrics> GetMetricsAsync(Guid organizationId, string accountType, CancellationToken cancellationToken);
}
