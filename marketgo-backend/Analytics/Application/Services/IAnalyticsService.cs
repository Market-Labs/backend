namespace marketgo_backend.Analytics.Application.Services;

public sealed record AnalyticsIndicator(string Label, decimal CurrentValue, decimal PreviousValue, string Unit);
public sealed record AnalyticsMetric(string Label, string Value);
public sealed record ReportSummary(string Title, string Description, IReadOnlyList<AnalyticsMetric> Metrics, IReadOnlyList<string> Highlights);
public sealed record AnalyticsResource(IReadOnlyList<AnalyticsIndicator> Indicators, IReadOnlyDictionary<string, ReportSummary> ReportSummaries);
public interface IAnalyticsService
{
    Task<AnalyticsResource> GetAsync(Guid organizationId, CancellationToken cancellationToken);
}
