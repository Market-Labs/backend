using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Iam.Interfaces.Rest.Resources;

namespace marketgo_backend.Iam.Interfaces.Rest.Transform;

public static class UserResourceFromEntityAssembler
{
    public static UserResource ToResourceFromEntity(User user)
    {
        var organizationId = user.OrganizationId;
        return new UserResource(user.Id.Value, user.FullName, user.Email,
            user.IsActive ? "active" : "inactive", [user.Role], [],
            user.AccountType == "supplier" ? organizationId : null,
            user.AccountType == "minimarket" ? organizationId : null,
            organizationId, user.Role, organizationId, user.OrganizationName, user.AccountType);
    }
}
