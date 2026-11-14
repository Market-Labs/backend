using marketgo_backend.Iam.Application.QueryServices;
using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Iam.Domain.Model.Queries;
using marketgo_backend.Iam.Domain.Repositories;

namespace marketgo_backend.Iam.Application.Internal.QueryServices;

public sealed class UserQueryService(IUserRepository users) : IUserQueryService
{
    public Task<User?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken) =>
        users.FindByIdAsNoTrackingAsync(query.Id, cancellationToken);

    public Task<IReadOnlyList<User>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken) =>
        users.ListByOrganizationAsync(query.OrganizationId, cancellationToken);

    public Task<User?> Handle(GetUserByUsernameQuery query, CancellationToken cancellationToken) =>
        users.FindByEmailAsync(query.Email.Trim().ToUpperInvariant(), cancellationToken);
}
