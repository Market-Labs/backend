using marketgo_backend.Profiles.Domain.Model.Aggregates;
using marketgo_backend.Profiles.Domain.Model.Queries;

namespace marketgo_backend.Profiles.Application.QueryServices;

public interface IProfileQueryService
{
    Task<IReadOnlyList<Profile>> Handle(GetAllProfilesQuery query, CancellationToken cancellationToken);
    Task<Profile?> Handle(GetProfileByEmailQuery query, CancellationToken cancellationToken);
    Task<Profile?> Handle(GetProfileByIdQuery query, CancellationToken cancellationToken);
}
