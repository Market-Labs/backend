using marketgo_backend.Organizations.Domain.Model.Aggregates;
using marketgo_backend.Organizations.Domain.Repositories;
using marketgo_backend.Shared.Infrastructure.Persistence.EFC.Configuration;
using marketgo_backend.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace marketgo_backend.Organizations.Infrastructure.Persistence.EFC.Repositories;

public sealed class OrganizationRepository(AppDbContext context)
    : BaseRepository<Organization, Guid>(context), IOrganizationRepository;
