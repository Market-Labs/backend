using marketgo_backend.Dashboard.Domain.Model.ValueObjects;
namespace marketgo_backend.Dashboard.Interfaces.Rest.Resources;
public sealed record DashboardResource(int Score, IReadOnlyList<DashboardIndicator> Indicators);
