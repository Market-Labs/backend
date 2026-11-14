using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Shared.Domain.Repositories;

namespace marketgo_backend.Iam.Domain.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default);
    new Task<User?> FindByIdAsNoTrackingAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> ListByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default);
}
