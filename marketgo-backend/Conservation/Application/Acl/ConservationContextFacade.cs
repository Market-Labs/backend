using marketgo_backend.Conservation.Domain.Repositories;
using marketgo_backend.Conservation.Interfaces.Acl;
namespace marketgo_backend.Conservation.Application.Acl;
public sealed class ConservationContextFacade(IConservationRepository repository) : IConservationContextFacade
{
    public async Task<ConservationMetrics> GetMetricsAsync(Guid id, CancellationToken ct)
    {
        var readings = await repository.ListRecentAsync(id, ct);
        return new(readings.Count, readings.Count(x => x.Status == "healthy"));
    }
}
