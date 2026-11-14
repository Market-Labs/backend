using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Profiles.Domain.Model.Aggregates;
using marketgo_backend.Profiles.Interfaces.Rest.Resources;

namespace marketgo_backend.Profiles.Interfaces.Rest.Transform;

public static class ProfileResourceFromEntityAssembler
{
    public static ProfileResource ToResourceFromEntity(Profile profile, User caller) =>
        new(profile.Id.Value,
            profile.OrganizationId == caller.OrganizationId && profile.OwnerEmail == caller.Email ? caller.Id.Value : 0,
            profile.OrganizationId,
            profile.Type == "supplier" ? "provider" : profile.Type,
            profile.BusinessName, profile.Email, profile.Phone, profile.Address,
            profile.District, profile.Specialty, profile.CoverageArea);
}
