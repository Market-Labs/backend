using marketgo_backend.Profiles.Domain.Model.Aggregates;
using marketgo_backend.Profiles.Domain.Model.ValueObjects;
using marketgo_backend.Profiles.Domain.Repositories;
using marketgo_backend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using marketgo_backend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace marketgo_backend.Profiles.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public sealed class ProfileRepository(AppDbContext context) : BaseRepository<Profile>(context), IProfileRepository
{
    public new Task<Profile?> FindByIdAsNoTrackingAsync(int id, CancellationToken cancellationToken = default) =>
        Context.Set<Profile>().AsNoTracking().FirstOrDefaultAsync(p => p.Id == new ProfileId(id), cancellationToken);

    public Task<Profile?> FindByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default) =>
        Context.Set<Profile>().FirstOrDefaultAsync(p => p.OrganizationId == organizationId, cancellationToken);

    public Task<Profile?> FindProfileByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        Context.Set<Profile>().FirstOrDefaultAsync(p => p.Email == email, cancellationToken);

    public Task<Profile?> FindProfileByEmailAsNoTrackingAsync(string email, CancellationToken cancellationToken = default) =>
        Context.Set<Profile>().AsNoTracking().FirstOrDefaultAsync(p => p.Email == email, cancellationToken);

    public async Task<IReadOnlyList<Profile>> ListPublicAsync(CancellationToken cancellationToken = default) =>
        await Context.Set<Profile>().AsNoTracking().OrderBy(p => p.BusinessName).ToListAsync(cancellationToken);
}
