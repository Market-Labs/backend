using marketgo_backend.Iam.Application.CommandServices;
using marketgo_backend.Iam.Application.QueryServices;
using marketgo_backend.Iam.Domain.Model.Commands;
using marketgo_backend.Iam.Domain.Model.Queries;
using marketgo_backend.Iam.Interfaces.Acl;

namespace marketgo_backend.Iam.Application.Acl;

public sealed class IamContextFacade(IUserCommandService commands, IUserQueryService queries) : IIamContextFacade
{
    public async Task<int> CreateUser(string fullName, string email, string password,
        string organizationName, string accountType, CancellationToken cancellationToken)
    {
        var result = await commands.Handle(new SignUpCommand(fullName, email, password,
            organizationName, accountType, true), cancellationToken);
        return result.IsSuccess ? result.Value!.user.Id.Value : 0;
    }

    public async Task<int> FetchUserIdByEmail(string email, CancellationToken cancellationToken)
    {
        var user = await queries.Handle(new GetUserByUsernameQuery(email), cancellationToken);
        return user?.Id.Value ?? 0;
    }

    public async Task<string> FetchEmailByUserId(int userId, CancellationToken cancellationToken)
    {
        var user = await queries.Handle(new GetUserByIdQuery(userId), cancellationToken);
        return user?.Email ?? string.Empty;
    }
}
