using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Iam.Domain.Model.Commands;
using marketgo_backend.Shared.Application.Model;

namespace marketgo_backend.Iam.Application.CommandServices;

public interface IUserCommandService
{
    Task<Result<(User user, string token)>> Handle(SignInCommand command, CancellationToken cancellationToken);
    Task<Result<(User user, string token)>> Handle(SignUpCommand command, CancellationToken cancellationToken);
}
