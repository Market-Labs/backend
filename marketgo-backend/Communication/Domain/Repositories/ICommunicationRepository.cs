using marketgo_backend.Communication.Domain.Model.Aggregates;
namespace marketgo_backend.Communication.Domain.Repositories;
public interface ICommunicationRepository
{
    Task<IReadOnlyList<Alert>> ListAlertsAsync(Guid organizationId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Message>> ListMessagesAsync(Guid organizationId, string box, CancellationToken cancellationToken);
    Task<Message?> FindByIdempotencyKeyAsync(Guid senderOrganizationId, Guid key, CancellationToken cancellationToken);
    Task<Alert?> FindAlertAsync(Guid id, Guid organizationId, CancellationToken cancellationToken);
    Task<Message?> FindMessageAsync(Guid id, Guid organizationId, CancellationToken cancellationToken);
    Task AddAlertAsync(Alert alert, CancellationToken cancellationToken);
    Task AddMessageAsync(Message message, CancellationToken cancellationToken);
}
