using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Iam.Interfaces.Rest.Resources;

namespace marketgo_backend.Iam.Interfaces.Rest.Transform;

public static class AuthenticatedUserResourceFromEntityAssembler
{
    public static AuthenticatedUserResource ToResourceFromEntity(User user, string token) =>
        new(token, UserResourceFromEntityAssembler.ToResourceFromEntity(user));
}
