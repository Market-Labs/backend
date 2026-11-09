using marketgo_backend.Profiles.Application.Services;
using marketgo_backend.Profiles.Domain.Model.Aggregates;
using marketgo_backend.Profiles.Domain.Model.ValueObjects;
using marketgo_backend.Profiles.Domain.Repositories;
using marketgo_backend.Shared.Application.Patterns;
using marketgo_backend.Shared.Domain.Repositories;

namespace marketgo_backend.Profiles.Application.Internal.CommandServices;

public sealed class ProfileService(IOrganizationRepository organizations, IUnitOfWork unitOfWork) : IProfileService
{
    public async Task<ProfileResource?> GetOwnAsync(Guid organizationId, Guid userId, CancellationToken cancellationToken = default)
    {
        var organization = await organizations.FindByIdAsync(organizationId, cancellationToken);
        return organization is null ? null : ToResource(organization, userId);
    }

    public async Task<Result<ProfileResource, ProfileError>> UpdateAsync(Guid organizationId, Guid userId, Guid profileId,
        UpdateProfileCommand command, CancellationToken cancellationToken = default)
    {
        if (organizationId != profileId)
            return Result<ProfileResource, ProfileError>.Fail(ProfileError.NotFound);
        var organization = await organizations.FindByIdAsync(profileId, cancellationToken);
        if (organization is null)
            return Result<ProfileResource, ProfileError>.Fail(ProfileError.NotFound);
        try
        {
            organization.UpdateContact(command.BusinessName, command.Email, command.Phone,
                command.Address, command.District, command.Specialty, command.CoverageArea);
        }
        catch (ArgumentException)
        {
            return Result<ProfileResource, ProfileError>.Fail(ProfileError.InvalidInput);
        }
        await unitOfWork.CompleteAsync(cancellationToken);
        return Result<ProfileResource, ProfileError>.Ok(ToResource(organization, userId));
    }

    private static ProfileResource ToResource(Organization organization, Guid userId) =>
        new(organization.Id, userId, organization.Kind == OrganizationKind.Supplier ? "provider" : "minimarket",
            organization.Name, organization.Email, organization.Phone, organization.Address,
            organization.District, organization.Specialty, organization.CoverageArea);
}
