namespace marketgo_backend.Dashboard.Application.Services;

public sealed record DashboardIndicator(string Title, string Type, decimal Value, int Variation = 0, string Severity = "normal");
public sealed record DashboardResource(int HealthScore, IReadOnlyList<DashboardIndicator> Indicators, IReadOnlyList<object> Activity);
public interface IDashboardService
{
    Task<DashboardResource> GetAsync(Guid organizationId, CancellationToken cancellationToken);
}
