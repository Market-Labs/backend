using marketgo_backend.Analytics.Domain.Model.Queries;
using marketgo_backend.Analytics.Domain.Model.ValueObjects;
namespace marketgo_backend.Analytics.Application.QueryServices;
public interface IAnalyticsQueryService
{
    Task<AnalyticsSnapshot> Handle(GetAnalyticsQuery query, CancellationToken cancellationToken);
}
