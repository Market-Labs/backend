using marketgo_backend.Profiles.Domain.Model.Aggregates;
using marketgo_backend.Profiles.Domain.Repositories;
using marketgo_backend.Profiles.Interfaces.Acl;

namespace marketgo_backend.Profiles.Application.Acl;

public sealed class ProfilesContextFacade(IProfileRepository profiles) : IProfilesContextFacade
{
    public Task<Profile?> FetchByOrganization(Guid organizationId, CancellationToken cancellationToken) =>
        profiles.FindByOrganizationAsync(organizationId, cancellationToken);
}
