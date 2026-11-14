namespace marketgo_backend.Communication.Interfaces.Acl;
public interface ICommunicationContextFacade
{
    Task<int> CountUnreadAlertsAsync(Guid organizationId, CancellationToken cancellationToken);
}
