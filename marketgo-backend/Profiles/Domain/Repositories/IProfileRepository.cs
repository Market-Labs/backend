using marketgo_backend.Profiles.Domain.Model.Aggregates;
using marketgo_backend.Shared.Domain.Repositories;

namespace marketgo_backend.Profiles.Domain.Repositories;

public interface IProfileRepository : IBaseRepository<Profile>
{
    new Task<Profile?> FindByIdAsNoTrackingAsync(int id, CancellationToken cancellationToken = default);
    Task<Profile?> FindByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<Profile?> FindProfileByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Profile?> FindProfileByEmailAsNoTrackingAsync(string email, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Profile>> ListPublicAsync(CancellationToken cancellationToken = default);
}
