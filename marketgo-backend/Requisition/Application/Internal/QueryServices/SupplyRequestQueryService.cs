using marketgo_backend.Requisition.Application.QueryServices;
using marketgo_backend.Requisition.Domain.Model.Aggregates;
using marketgo_backend.Requisition.Domain.Model.Queries;
using marketgo_backend.Requisition.Domain.Repositories;
namespace marketgo_backend.Requisition.Application.Internal.QueryServices;
public sealed class SupplyRequestQueryService(ISupplyRequestRepository requests) : ISupplyRequestQueryService
{
    public Task<IReadOnlyList<SupplyRequest>> Handle(GetSupplyRequestsQuery query, CancellationToken ct) =>
        requests.ListForAsync(query.OrganizationId, query.AccountType, ct);
    public async Task<SupplyRequest?> FindVisibleAsync(Guid id, Guid organizationId, string accountType, CancellationToken ct)
    {
        var request = await requests.FindAsync(id, ct);
        return request is not null &&
            (accountType == "supplier" ? request.SupplierId == organizationId : request.MinimarketId == organizationId)
            ? request : null;
    }
}
