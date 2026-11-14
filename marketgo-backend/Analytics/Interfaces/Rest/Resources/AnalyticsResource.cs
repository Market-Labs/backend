using marketgo_backend.Analytics.Domain.Model.ValueObjects;
namespace marketgo_backend.Analytics.Interfaces.Rest.Resources;
public sealed record AnalyticsResource(IReadOnlyList<OperationalIndicator> Indicators,
    IReadOnlyDictionary<string, ReportSummary> ReportSummaries);
