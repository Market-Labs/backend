using marketgo_backend.Iam.Domain.Model.Commands;
using marketgo_backend.Iam.Interfaces.Rest.Resources;

namespace marketgo_backend.Iam.Interfaces.Rest.Transform;

public static class SignUpCommandFromResourceAssembler
{
    public static SignUpCommand ToCommandFromResource(SignUpResource resource) =>
        new(resource.FullName, resource.Email, resource.Password, resource.OrganizationName,
            resource.AccountType, resource.AcceptedTerms);
}
