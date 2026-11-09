using marketgo_backend.Shared.Application.Patterns;

namespace marketgo_backend.Profiles.Application.Services;

public interface IProfileService
{
    Task<ProfileResource?> GetOwnAsync(Guid organizationId, Guid userId, CancellationToken cancellationToken = default);
    Task<Result<ProfileResource, ProfileError>> UpdateAsync(Guid organizationId, Guid userId, Guid profileId,
        UpdateProfileCommand command, CancellationToken cancellationToken = default);
}
