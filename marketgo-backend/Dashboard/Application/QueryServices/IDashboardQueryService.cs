using marketgo_backend.Dashboard.Domain.Model.Queries;
using marketgo_backend.Dashboard.Domain.Model.ValueObjects;
namespace marketgo_backend.Dashboard.Application.QueryServices;
public interface IDashboardQueryService
{
    Task<DashboardSnapshot> Handle(GetDashboardQuery query, CancellationToken cancellationToken);
}
