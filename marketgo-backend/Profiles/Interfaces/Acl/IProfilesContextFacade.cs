using marketgo_backend.Profiles.Domain.Model.Aggregates;

namespace marketgo_backend.Profiles.Interfaces.Acl;

public interface IProfilesContextFacade
{
    Task<Profile?> FetchByOrganization(Guid organizationId, CancellationToken cancellationToken);
}
