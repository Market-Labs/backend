using System.ComponentModel.DataAnnotations;
using marketgo_backend.Iam.Application.Commands;
using marketgo_backend.Iam.Application.Errors;
using marketgo_backend.Iam.Application.Services;
using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Iam.Domain.Repositories;
using marketgo_backend.Organizations.Domain.Model.Aggregates;
using marketgo_backend.Organizations.Domain.Model.ValueObjects;
using marketgo_backend.Organizations.Domain.Repositories;
using marketgo_backend.Shared.Application.Patterns;
using marketgo_backend.Shared.Domain.Repositories;

namespace marketgo_backend.Iam.Application.Internal.CommandServices;

public sealed class AuthenticationService(
    IUserRepository users,
    ISessionRepository sessions,
    IOrganizationRepository organizations,
    IPasswordHasher passwords,
    ISessionTokenService tokens,
    IUnitOfWork unitOfWork,
    TimeProvider clock) : IAuthenticationService
{
    private static readonly TimeSpan SessionLifetime = TimeSpan.FromHours(12);

    public async Task<Result<AuthenticatedSession, AuthenticationError>> SignUpAsync(
        SignUpCommand command, CancellationToken cancellationToken = default)
    {
        if (!IsValidSignUp(command))
            return Result<AuthenticatedSession, AuthenticationError>.Fail(AuthenticationError.InvalidInput);

        var normalizedEmail = command.Email.Trim().ToUpperInvariant();
        if (await users.FindByEmailAsync(normalizedEmail, cancellationToken) is not null)
            return Result<AuthenticatedSession, AuthenticationError>.Fail(AuthenticationError.EmailAlreadyRegistered);

        var organization = new Organization(command.OrganizationName, command.OrganizationKind);
        var user = new User(organization.Id, command.FullName, command.Email, passwords.Hash(command.Password));
        var now = clock.GetUtcNow();
        user.RecordSignIn(now);
        var token = tokens.CreateToken();
        var session = new Session(user.Id, tokens.HashToken(token), now, now.Add(SessionLifetime));

        await organizations.AddAsync(organization, cancellationToken);
        await users.AddAsync(user, cancellationToken);
        await sessions.AddAsync(session, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);

        return Result<AuthenticatedSession, AuthenticationError>.Ok(
            new AuthenticatedSession(user, organization, token, session.ExpiresAt));
    }

    public async Task<Result<AuthenticatedSession, AuthenticationError>> SignInAsync(
        SignInCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Email) || string.IsNullOrEmpty(command.Password))
            return Result<AuthenticatedSession, AuthenticationError>.Fail(AuthenticationError.InvalidCredentials);

        var user = await users.FindByEmailAsync(command.Email.Trim().ToUpperInvariant(), cancellationToken);
        if (user is null || !user.IsActive || !passwords.Verify(command.Password, user.PasswordHash))
            return Result<AuthenticatedSession, AuthenticationError>.Fail(AuthenticationError.InvalidCredentials);

        var organization = await organizations.FindByIdAsync(user.OrganizationId, cancellationToken);
        if (organization is null)
            return Result<AuthenticatedSession, AuthenticationError>.Fail(AuthenticationError.InvalidCredentials);

        var now = clock.GetUtcNow();
        user.RecordSignIn(now);
        var token = tokens.CreateToken();
        var session = new Session(user.Id, tokens.HashToken(token), now, now.Add(SessionLifetime));
        await sessions.AddAsync(session, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);

        return Result<AuthenticatedSession, AuthenticationError>.Ok(
            new AuthenticatedSession(user, organization, token, session.ExpiresAt));
    }

    public async Task RevokeSessionAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var session = await sessions.FindByIdAsync(sessionId, cancellationToken);
        if (session is null) return;
        session.Revoke(clock.GetUtcNow());
        await unitOfWork.CompleteAsync(cancellationToken);
    }

    private static bool IsValidSignUp(SignUpCommand command) =>
        !string.IsNullOrWhiteSpace(command.FullName) && command.FullName.Trim().Length <= 160 &&
        !string.IsNullOrWhiteSpace(command.Email) && command.Email.Trim().Length <= 254 &&
        new EmailAddressAttribute().IsValid(command.Email) &&
        command.Password is { Length: >= 8 and <= 128 } &&
        !string.IsNullOrWhiteSpace(command.OrganizationName) && command.OrganizationName.Trim().Length <= 160 &&
        command.OrganizationKind is OrganizationKind.Minimarket or OrganizationKind.Supplier &&
        command.AcceptedTerms;
}
