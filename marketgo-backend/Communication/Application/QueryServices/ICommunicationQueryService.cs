using marketgo_backend.Communication.Domain.Model.Aggregates;
using marketgo_backend.Communication.Domain.Model.Queries;
namespace marketgo_backend.Communication.Application.QueryServices;
public interface ICommunicationQueryService
{
    Task<IReadOnlyList<Alert>> Alerts(GetCommunicationQuery query, CancellationToken cancellationToken);
    Task<IReadOnlyList<Message>> Messages(GetCommunicationQuery query, CancellationToken cancellationToken);
}
