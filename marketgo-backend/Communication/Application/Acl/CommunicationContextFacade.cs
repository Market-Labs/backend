using marketgo_backend.Communication.Domain.Repositories;
using marketgo_backend.Communication.Interfaces.Acl;
namespace marketgo_backend.Communication.Application.Acl;
public sealed class CommunicationContextFacade(ICommunicationRepository repository) : ICommunicationContextFacade
{
    public async Task<int> CountUnreadAlertsAsync(Guid id, CancellationToken ct) =>
        (await repository.ListAlertsAsync(id, ct)).Count(x => !x.Read);
}
