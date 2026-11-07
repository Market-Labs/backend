using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Shared.Domain.Repositories;

namespace marketgo_backend.Iam.Domain.Repositories;

public interface IUserRepository : IBaseRepository<User, Guid>
{
    Task<User?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default);
}
