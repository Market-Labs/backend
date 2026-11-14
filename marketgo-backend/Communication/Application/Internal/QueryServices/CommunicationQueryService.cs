using marketgo_backend.Communication.Application.QueryServices;
using marketgo_backend.Communication.Domain.Model.Aggregates;
using marketgo_backend.Communication.Domain.Model.Queries;
using marketgo_backend.Communication.Domain.Repositories;
namespace marketgo_backend.Communication.Application.Internal.QueryServices;
public sealed class CommunicationQueryService(ICommunicationRepository repository) : ICommunicationQueryService
{
    public Task<IReadOnlyList<Alert>> Alerts(GetCommunicationQuery query, CancellationToken ct) =>
        repository.ListAlertsAsync(query.OrganizationId, ct);
    public Task<IReadOnlyList<Message>> Messages(GetCommunicationQuery query, CancellationToken ct) =>
        repository.ListMessagesAsync(query.OrganizationId, ct);
}
