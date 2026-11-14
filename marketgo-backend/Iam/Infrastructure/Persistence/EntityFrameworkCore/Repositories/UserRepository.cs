using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Iam.Domain.Model.ValueObjects;
using marketgo_backend.Iam.Domain.Repositories;
using marketgo_backend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using marketgo_backend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace marketgo_backend.Iam.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public sealed class UserRepository(AppDbContext context) : BaseRepository<User>(context), IUserRepository
{
    public Task<User?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default) =>
        Context.Set<User>().FirstOrDefaultAsync(user => user.NormalizedEmail == normalizedEmail, cancellationToken);

    public new Task<User?> FindByIdAsNoTrackingAsync(int id, CancellationToken cancellationToken = default) =>
        Context.Set<User>().AsNoTracking().FirstOrDefaultAsync(user => user.Id == new UserId(id), cancellationToken);

    public async Task<IReadOnlyList<User>> ListByOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default) =>
        await Context.Set<User>().AsNoTracking().Where(user => user.OrganizationId == organizationId)
            .OrderBy(user => user.FullName).ToListAsync(cancellationToken);

    public Task<bool> ExistsByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default) =>
        Context.Set<User>().AnyAsync(user => user.NormalizedEmail == normalizedEmail, cancellationToken);
}
