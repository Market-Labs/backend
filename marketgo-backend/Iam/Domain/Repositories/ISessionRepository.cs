using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Shared.Domain.Repositories;

namespace marketgo_backend.Iam.Domain.Repositories;

public interface ISessionRepository : IBaseRepository<Session, Guid>
{
    Task<Session?> FindActiveByTokenHashAsync(string tokenHash, DateTimeOffset now, CancellationToken cancellationToken = default);
}
