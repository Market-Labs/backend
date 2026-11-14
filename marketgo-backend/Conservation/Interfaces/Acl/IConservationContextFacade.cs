namespace marketgo_backend.Conservation.Interfaces.Acl;
public sealed record ConservationMetrics(int TotalReadings, int HealthyReadings);
public interface IConservationContextFacade
{
    Task<ConservationMetrics> GetMetricsAsync(Guid organizationId, CancellationToken cancellationToken);
}
