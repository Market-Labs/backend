using System.ComponentModel.DataAnnotations;
using marketgo_backend.Iam.Application.CommandServices;
using marketgo_backend.Iam.Application.Internal.OutboundServices;
using marketgo_backend.Iam.Domain.Model;
using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Iam.Domain.Model.Commands;
using marketgo_backend.Iam.Domain.Model.Errors;
using marketgo_backend.Iam.Domain.Repositories;
using marketgo_backend.Shared.Application.Model;
using marketgo_backend.Shared.Domain.Repositories;

namespace marketgo_backend.Iam.Application.Internal.CommandServices;

public sealed class UserCommandService(IUserRepository users, ITokenService tokens,
    IHashingService hashing, IUnitOfWork unitOfWork, TimeProvider clock) : IUserCommandService
{
    public async Task<Result<(User user, string token)>> Handle(SignInCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Email) || string.IsNullOrEmpty(command.Password))
            return Result<(User user, string token)>.Failure(IamErrors.InvalidCredentials, IamError.InvalidCredentials);
        var user = await users.FindByEmailAsync(command.Email.Trim().ToUpperInvariant(), cancellationToken);
        if (user is null || !user.IsActive || !hashing.VerifyPassword(command.Password, user.PasswordHash))
            return Result<(User user, string token)>.Failure(IamErrors.InvalidCredentials, IamError.InvalidCredentials);
        user.RecordSignIn(clock.GetUtcNow());
        await unitOfWork.CompleteAsync(cancellationToken);
        return Result<(User user, string token)>.Success((user, tokens.GenerateToken(user)));
    }

    public async Task<Result<(User user, string token)>> Handle(SignUpCommand command, CancellationToken cancellationToken)
    {
        if (!Valid(command))
            return Result<(User user, string token)>.Failure(IamErrors.InvalidRegistration, IamError.InvalidRegistration);
        var normalizedEmail = command.Email.Trim().ToUpperInvariant();
        if (await users.ExistsByEmailAsync(normalizedEmail, cancellationToken))
            return Result<(User user, string token)>.Failure(IamErrors.EmailAlreadyTaken, IamError.EmailAlreadyTaken);
        var user = new User(command.FullName, command.Email, hashing.HashPassword(command.Password),
            command.OrganizationName, command.AccountType);
        await users.AddAsync(user, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return Result<(User user, string token)>.Success((user, tokens.GenerateToken(user)));
    }

    private static bool Valid(SignUpCommand command) =>
        !string.IsNullOrWhiteSpace(command.FullName) && command.FullName.Trim().Length <= 160 &&
        !string.IsNullOrWhiteSpace(command.Email) && command.Email.Length <= 254 &&
        new EmailAddressAttribute().IsValid(command.Email) &&
        command.Password is { Length: >= 8 and <= 128 } &&
        !string.IsNullOrWhiteSpace(command.OrganizationName) && command.OrganizationName.Trim().Length <= 160 &&
        command.AccountType is "minimarket" or "supplier" && command.AcceptedTerms;
}
