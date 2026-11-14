namespace marketgo_backend.Analytics.Domain.Model.ValueObjects;
public sealed record ReportMetric(string Label, string Value);
public sealed record ReportSummary(string Title, string Description, IReadOnlyList<ReportMetric> Metrics);
