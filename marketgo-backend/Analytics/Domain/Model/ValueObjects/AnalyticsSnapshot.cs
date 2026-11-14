namespace marketgo_backend.Analytics.Domain.Model.ValueObjects;
public sealed record AnalyticsSnapshot(IReadOnlyList<OperationalIndicator> Indicators,
    IReadOnlyDictionary<string, ReportSummary> ReportSummaries);
