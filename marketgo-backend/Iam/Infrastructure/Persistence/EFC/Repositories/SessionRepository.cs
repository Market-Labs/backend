using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Iam.Domain.Repositories;
using marketgo_backend.Shared.Infrastructure.Persistence.EFC.Configuration;
using marketgo_backend.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace marketgo_backend.Iam.Infrastructure.Persistence.EFC.Repositories;

public sealed class SessionRepository(AppDbContext context) : BaseRepository<Session, Guid>(context), ISessionRepository
{
    public Task<Session?> FindActiveByTokenHashAsync(string tokenHash, DateTimeOffset now, CancellationToken cancellationToken = default) =>
        Context.Set<Session>().AsNoTracking().SingleOrDefaultAsync(
            session => session.TokenHash == tokenHash && session.RevokedAt == null && session.ExpiresAt > now,
            cancellationToken);
}
