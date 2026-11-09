using marketgo_backend.Profiles.Domain.Model.Aggregates;
using marketgo_backend.Profiles.Domain.Repositories;
using marketgo_backend.Shared.Infrastructure.Persistence.EFC.Configuration;
using marketgo_backend.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace marketgo_backend.Profiles.Infrastructure.Persistence.EFC.Repositories;

public sealed class OrganizationRepository(AppDbContext context)
    : BaseRepository<Organization, Guid>(context), IOrganizationRepository;
