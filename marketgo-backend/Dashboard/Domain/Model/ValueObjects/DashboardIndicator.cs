namespace marketgo_backend.Dashboard.Domain.Model.ValueObjects;
public sealed record DashboardIndicator(string Title, string Type, int Value,
    int Variation = 0, string Severity = "normal");
