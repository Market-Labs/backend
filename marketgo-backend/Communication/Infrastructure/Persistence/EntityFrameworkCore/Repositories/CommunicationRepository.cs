using marketgo_backend.Communication.Domain.Model.Aggregates;
using marketgo_backend.Communication.Domain.Repositories;
using marketgo_backend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore;
namespace marketgo_backend.Communication.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
public sealed class CommunicationRepository(AppDbContext context) : ICommunicationRepository
{
    public async Task<IReadOnlyList<Alert>> ListAlertsAsync(Guid id, CancellationToken ct) =>
        await context.Set<Alert>().AsNoTracking().Where(x => x.OrganizationId == id)
            .OrderByDescending(x => x.SentAt).Take(500).ToListAsync(ct);
    public async Task<IReadOnlyList<Message>> ListMessagesAsync(Guid id, string box, CancellationToken ct)
    {
        var query = context.Set<Message>().AsNoTracking();
        query = box switch
        {
            "sent" => query.Where(x => x.SenderOrganizationId == id ||
                (x.SenderOrganizationId == null && x.OrganizationId == id)),
            "inbox" => query.Where(x => x.ReceiverOrganizationId == id),
            _ => query.Where(x => x.SenderOrganizationId == id || x.ReceiverOrganizationId == id ||
                (x.SenderOrganizationId == null && x.OrganizationId == id))
        };
        return await query.OrderByDescending(x => x.SentAt).Take(500).ToListAsync(ct);
    }
    public Task<Message?> FindByIdempotencyKeyAsync(Guid senderOrganizationId, Guid key, CancellationToken ct) =>
        context.Set<Message>().AsNoTracking().FirstOrDefaultAsync(x =>
            x.SenderOrganizationId == senderOrganizationId && x.IdempotencyKey == key, ct);
    public Task<Alert?> FindAlertAsync(Guid id, Guid org, CancellationToken ct) =>
        context.Set<Alert>().FirstOrDefaultAsync(x => x.Id == id && x.OrganizationId == org, ct);
    public Task<Message?> FindMessageAsync(Guid id, Guid org, CancellationToken ct) =>
        context.Set<Message>().FirstOrDefaultAsync(x => x.Id == id &&
            (x.SenderOrganizationId == org || x.ReceiverOrganizationId == org ||
             (x.SenderOrganizationId == null && x.OrganizationId == org)), ct);
    public async Task AddAlertAsync(Alert alert, CancellationToken ct) => await context.Set<Alert>().AddAsync(alert, ct);
    public async Task AddMessageAsync(Message message, CancellationToken ct) => await context.Set<Message>().AddAsync(message, ct);
}
