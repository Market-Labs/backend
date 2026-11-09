using marketgo_backend.Iam.Application.Commands;
using marketgo_backend.Iam.Application.Errors;
using marketgo_backend.Shared.Application.Patterns;

namespace marketgo_backend.Iam.Application.Services;

public interface IAuthenticationService
{
    Task<Result<AuthenticatedSession, AuthenticationError>> SignUpAsync(
        SignUpCommand command, CancellationToken cancellationToken = default);

    Task<Result<AuthenticatedSession, AuthenticationError>> SignInAsync(
        SignInCommand command, CancellationToken cancellationToken = default);

    Task RevokeSessionAsync(Guid sessionId, CancellationToken cancellationToken = default);
}
