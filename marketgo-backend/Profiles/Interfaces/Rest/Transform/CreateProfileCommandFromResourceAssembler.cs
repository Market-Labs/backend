using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Profiles.Domain.Model.Commands;
using marketgo_backend.Profiles.Interfaces.Rest.Resources;

namespace marketgo_backend.Profiles.Interfaces.Rest.Transform;

public static class CreateProfileCommandFromResourceAssembler
{
    public static CreateProfileCommand ToCommandFromResource(CreateProfileResource resource, User caller) =>
        new(caller.OrganizationId, caller.Email, caller.AccountType, resource.BusinessName, resource.Email,
            resource.Phone, resource.Address, resource.District, resource.Specialty, resource.CoverageArea);
}
