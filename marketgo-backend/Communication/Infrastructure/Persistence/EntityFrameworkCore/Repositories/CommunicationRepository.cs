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
    public async Task<IReadOnlyList<Message>> ListMessagesAsync(Guid id, CancellationToken ct) =>
        await context.Set<Message>().AsNoTracking().Where(x => x.OrganizationId == id)
            .OrderByDescending(x => x.SentAt).Take(500).ToListAsync(ct);
    public Task<Alert?> FindAlertAsync(Guid id, Guid org, CancellationToken ct) =>
        context.Set<Alert>().FirstOrDefaultAsync(x => x.Id == id && x.OrganizationId == org, ct);
    public Task<Message?> FindMessageAsync(Guid id, Guid org, CancellationToken ct) =>
        context.Set<Message>().FirstOrDefaultAsync(x => x.Id == id && x.OrganizationId == org, ct);
    public async Task AddAlertAsync(Alert alert, CancellationToken ct) => await context.Set<Alert>().AddAsync(alert, ct);
    public async Task AddMessageAsync(Message message, CancellationToken ct) => await context.Set<Message>().AddAsync(message, ct);
}
