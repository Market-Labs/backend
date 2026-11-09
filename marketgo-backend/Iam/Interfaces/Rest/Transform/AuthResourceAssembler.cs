using marketgo_backend.Iam.Application.Services;
using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Iam.Interfaces.Rest.Resources;
using marketgo_backend.Profiles.Domain.Model.Aggregates;

namespace marketgo_backend.Iam.Interfaces.Rest.Transform;

public static class AuthResourceAssembler
{
    public static AuthenticatedUserResource ToResource(AuthenticatedSession session) =>
        new(session.Token, "Bearer", session.ExpiresAt, ToResource(session.User, session.Organization));

    public static UserResource ToResource(User user, Organization organization) =>
        new(user.Id, user.FullName, user.Email, user.Role, organization.Id,
            organization.Name, organization.Kind.ToString().ToLowerInvariant(), user.LastSignedInAt);
}
