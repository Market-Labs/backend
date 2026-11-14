using marketgo_backend.Dashboard.Domain.Model.ValueObjects;
using marketgo_backend.Dashboard.Interfaces.Rest.Resources;
namespace marketgo_backend.Dashboard.Interfaces.Rest.Transform;
public static class DashboardResourceFromSnapshotAssembler
{
    public static DashboardResource ToResource(DashboardSnapshot snapshot) =>
        new(snapshot.Score, snapshot.Indicators);
}
