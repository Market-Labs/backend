using marketgo_backend.Analytics.Domain.Model.ValueObjects;
using marketgo_backend.Analytics.Interfaces.Rest.Resources;
namespace marketgo_backend.Analytics.Interfaces.Rest.Transform;
public static class AnalyticsResourceFromSnapshotAssembler
{
    public static AnalyticsResource ToResource(AnalyticsSnapshot snapshot) =>
        new(snapshot.Indicators, snapshot.ReportSummaries);
}
