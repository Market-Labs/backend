using marketgo_backend.Profiles.Application.QueryServices;
using marketgo_backend.Profiles.Domain.Model.Aggregates;
using marketgo_backend.Profiles.Domain.Model.Queries;
using marketgo_backend.Profiles.Domain.Repositories;

namespace marketgo_backend.Profiles.Application.Internal.QueryServices;

public sealed class ProfileQueryService(IProfileRepository profiles) : IProfileQueryService
{
    public Task<IReadOnlyList<Profile>> Handle(GetAllProfilesQuery query, CancellationToken cancellationToken) =>
        profiles.ListPublicAsync(cancellationToken);

    public Task<Profile?> Handle(GetProfileByEmailQuery query, CancellationToken cancellationToken) =>
        profiles.FindProfileByEmailAsNoTrackingAsync(query.Email.Trim().ToLowerInvariant(), cancellationToken);

    public Task<Profile?> Handle(GetProfileByIdQuery query, CancellationToken cancellationToken) =>
        profiles.FindByIdAsNoTrackingAsync(query.ProfileId, cancellationToken);
}
