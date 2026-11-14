using marketgo_backend.Profiles.Application.CommandServices;
using marketgo_backend.Profiles.Domain.Model;
using marketgo_backend.Profiles.Domain.Model.Aggregates;
using marketgo_backend.Profiles.Domain.Model.Commands;
using marketgo_backend.Profiles.Domain.Model.Errors;
using marketgo_backend.Profiles.Domain.Repositories;
using marketgo_backend.Shared.Application.Model;
using marketgo_backend.Shared.Domain.Repositories;

namespace marketgo_backend.Profiles.Application.Internal.CommandServices;

public sealed class ProfileCommandService(IProfileRepository profiles, IUnitOfWork unitOfWork) : IProfileCommandService
{
    public async Task<Result<Profile>> Handle(CreateProfileCommand command, CancellationToken cancellationToken)
    {
        if (await profiles.FindByOrganizationAsync(command.OrganizationId, cancellationToken) is not null)
            return Result<Profile>.Failure(ProfileErrors.ProfileAlreadyExists, ProfilesError.ProfileAlreadyExists);
        try
        {
            var profile = new Profile(command);
            await profiles.AddAsync(profile, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<Profile>.Success(profile);
        }
        catch (ArgumentException)
        {
            return Result<Profile>.Failure(ProfileErrors.InvalidProfileData, ProfilesError.InvalidProfileData);
        }
    }

    public async Task<Result<Profile>> Handle(UpdateProfileCommand command, CancellationToken cancellationToken)
    {
        var profile = await profiles.FindByIdAsync(command.ProfileId, cancellationToken);
        if (profile is null) return Result<Profile>.Failure(ProfileErrors.ProfileNotFound, ProfilesError.ProfileNotFound);
        if (profile.OrganizationId != command.OrganizationId)
            return Result<Profile>.Failure(ProfileErrors.Forbidden, ProfilesError.Forbidden);
        try { profile.Update(command); }
        catch (ArgumentException)
        {
            return Result<Profile>.Failure(ProfileErrors.InvalidProfileData, ProfilesError.InvalidProfileData);
        }
        await unitOfWork.CompleteAsync(cancellationToken);
        return Result<Profile>.Success(profile);
    }
}
