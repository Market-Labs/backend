using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Iam.Domain.Repositories;
using marketgo_backend.Shared.Infrastructure.Persistence.EFC.Configuration;
using marketgo_backend.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace marketgo_backend.Iam.Infrastructure.Persistence.EFC.Repositories;

public sealed class UserRepository(AppDbContext context) : BaseRepository<User, Guid>(context), IUserRepository
{
    public Task<User?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default) =>
        Context.Set<User>().SingleOrDefaultAsync(user => user.NormalizedEmail == normalizedEmail, cancellationToken);
}
