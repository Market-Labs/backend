using marketgo_backend.Profiles.Domain.Model.Aggregates;
using marketgo_backend.Shared.Domain.Repositories;

namespace marketgo_backend.Profiles.Domain.Repositories;

public interface IOrganizationRepository : IBaseRepository<Organization, Guid>;
