using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Profiles.Domain.Model.Commands;
using marketgo_backend.Profiles.Interfaces.Rest.Resources;

namespace marketgo_backend.Profiles.Interfaces.Rest.Transform;

public static class UpdateProfileCommandFromResourceAssembler
{
    public static UpdateProfileCommand ToCommandFromResource(int id, UpdateProfileResource resource, User caller) =>
        new(id, caller.OrganizationId, resource.BusinessName, resource.Email, resource.Phone,
            resource.Address, resource.District, resource.Specialty, resource.CoverageArea);
}
