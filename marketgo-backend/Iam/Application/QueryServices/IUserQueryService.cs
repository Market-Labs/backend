using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Iam.Domain.Model.Queries;

namespace marketgo_backend.Iam.Application.QueryServices;

public interface IUserQueryService
{
    Task<User?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken);
    Task<IReadOnlyList<User>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken);
    Task<User?> Handle(GetUserByUsernameQuery query, CancellationToken cancellationToken);
}
