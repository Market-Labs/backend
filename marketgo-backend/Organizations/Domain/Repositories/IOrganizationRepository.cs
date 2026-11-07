using marketgo_backend.Organizations.Domain.Model.Aggregates;
using marketgo_backend.Shared.Domain.Repositories;

namespace marketgo_backend.Organizations.Domain.Repositories;

public interface IOrganizationRepository : IBaseRepository<Organization, Guid>;
