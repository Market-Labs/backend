using marketgo_backend.Iam.Domain.Model.Commands;
using marketgo_backend.Iam.Interfaces.Rest.Resources;

namespace marketgo_backend.Iam.Interfaces.Rest.Transform;

public static class SignInCommandFromResourceAssembler
{
    public static SignInCommand ToCommandFromResource(SignInResource resource) =>
        new(resource.Email, resource.Password);
}
