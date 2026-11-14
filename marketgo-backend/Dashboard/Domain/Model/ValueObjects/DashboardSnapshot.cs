namespace marketgo_backend.Dashboard.Domain.Model.ValueObjects;
public sealed record DashboardSnapshot(int Score, IReadOnlyList<DashboardIndicator> Indicators);
