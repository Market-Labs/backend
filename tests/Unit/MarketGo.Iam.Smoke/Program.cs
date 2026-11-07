using marketgo_backend.Iam.Application.Commands;
using marketgo_backend.Iam.Application.Errors;
using marketgo_backend.Iam.Application.Internal.CommandServices;
using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Iam.Domain.Repositories;
using marketgo_backend.Iam.Infrastructure.Security;
using marketgo_backend.Organizations.Domain.Model.Aggregates;
using marketgo_backend.Organizations.Domain.Model.ValueObjects;
using marketgo_backend.Organizations.Domain.Repositories;
using marketgo_backend.Shared.Application.Patterns;
using marketgo_backend.Shared.Domain.Repositories;
using marketgo_backend.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.EntityFrameworkCore;

var users = new UserStore();
var sessions = new SessionStore();
var organizations = new OrganizationStore();
var passwords = new Pbkdf2PasswordHasher();
var tokens = new SessionTokenService();
var unitOfWork = new UnitOfWorkStub();
var auth = new AuthenticationService(users, sessions, organizations, passwords, tokens, unitOfWork, TimeProvider.System);

var invalid = await auth.SignUpAsync(new SignUpCommand("Ana", "ana@example.com", "password123", "Mi tienda", OrganizationKind.Minimarket, false));
Check(invalid is Result<marketgo_backend.Iam.Application.Services.AuthenticatedSession, AuthenticationError>.Failure { Error: AuthenticationError.InvalidInput }, "Registration requires accepted terms");

var signedUp = await auth.SignUpAsync(new SignUpCommand("Ana", "ana@example.com", "password123", "Mi tienda", OrganizationKind.Minimarket, true));
Check(signedUp is Result<marketgo_backend.Iam.Application.Services.AuthenticatedSession, AuthenticationError>.Success, "Registration succeeds");
var first = ((Result<marketgo_backend.Iam.Application.Services.AuthenticatedSession, AuthenticationError>.Success)signedUp).Value;
Check(first.User.OrganizationId == first.Organization.Id, "User belongs to organization");
Check(first.Organization.Kind == OrganizationKind.Minimarket, "Organization kind is saved");
Check(first.User.PasswordHash != "password123" && passwords.Verify("password123", first.User.PasswordHash), "Password is hashed");
Check(first.Token.Length == 43 && sessions.Items.Single().TokenHash == tokens.HashToken(first.Token), "Only token hash is persisted");
Check(unitOfWork.Saves == 1, "Registration saves once");

var duplicate = await auth.SignUpAsync(new SignUpCommand("Another", "ANA@example.com", "password123", "Other", OrganizationKind.Supplier, true));
Check(duplicate is Result<marketgo_backend.Iam.Application.Services.AuthenticatedSession, AuthenticationError>.Failure { Error: AuthenticationError.EmailAlreadyRegistered }, "Email is unique regardless of case");
var wrong = await auth.SignInAsync(new SignInCommand("ana@example.com", "wrong"));
Check(wrong is Result<marketgo_backend.Iam.Application.Services.AuthenticatedSession, AuthenticationError>.Failure { Error: AuthenticationError.InvalidCredentials }, "Wrong password is rejected");
var signedIn = await auth.SignInAsync(new SignInCommand("ANA@example.com", "password123"));
Check(signedIn is Result<marketgo_backend.Iam.Application.Services.AuthenticatedSession, AuthenticationError>.Success, "Correct password signs in");
var second = ((Result<marketgo_backend.Iam.Application.Services.AuthenticatedSession, AuthenticationError>.Success)signedIn).Value;
Check(second.Token != first.Token && sessions.Items.Count == 2, "Sign-in creates a new session");
var active = await sessions.FindActiveByTokenHashAsync(tokens.HashToken(second.Token), TimeProvider.System.GetUtcNow());
Check(active is not null, "New session is active");
await auth.RevokeSessionAsync(active!.Id);
Check(await sessions.FindActiveByTokenHashAsync(tokens.HashToken(second.Token), TimeProvider.System.GetUtcNow()) is null, "Sign-out revokes session");
Check(unitOfWork.Saves == 3, "Registration, sign-in and sign-out save");
using (var db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseMySQL("server=localhost;database=marketgo;user=test;password=test").Options))
{
    Check(db.Model.FindEntityType(typeof(User))?.GetTableName() == "users", "User EF mapping is registered");
    Check(db.Model.FindEntityType(typeof(Session))?.GetTableName() == "sessions", "Session EF mapping is registered");
    Check(db.Model.FindEntityType(typeof(Organization))?.GetTableName() == "organizations", "Organization EF mapping is registered");
}
Console.WriteLine("IAM smoke tests passed (registration, validation, sign-in, session, sign-out and EF model).");

static void Check(bool condition, string message)
{
    if (!condition) throw new Exception(message);
}

abstract class Store<TEntity> : IBaseRepository<TEntity, Guid> where TEntity : class
{
    public List<TEntity> Items { get; } = [];
    public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) { Items.Add(entity); return Task.CompletedTask; }
    public abstract Task<TEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public Task<IReadOnlyList<TEntity>> ListAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<TEntity>>(Items);
    public void Update(TEntity entity) { }
    public void Remove(TEntity entity) => Items.Remove(entity);
}
sealed class UserStore : Store<User>, IUserRepository
{
    public override Task<User?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(Items.SingleOrDefault(x => x.Id == id));
    public Task<User?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default) => Task.FromResult(Items.SingleOrDefault(x => x.NormalizedEmail == normalizedEmail));
}
sealed class SessionStore : Store<Session>, ISessionRepository
{
    public override Task<Session?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(Items.SingleOrDefault(x => x.Id == id));
    public Task<Session?> FindActiveByTokenHashAsync(string tokenHash, DateTimeOffset now, CancellationToken cancellationToken = default) => Task.FromResult(Items.SingleOrDefault(x => x.TokenHash == tokenHash && x.IsActiveAt(now)));
}
sealed class OrganizationStore : Store<Organization>, IOrganizationRepository
{
    public override Task<Organization?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(Items.SingleOrDefault(x => x.Id == id));
}
sealed class UnitOfWorkStub : IUnitOfWork
{
    public int Saves { get; private set; }
    public Task CompleteAsync(CancellationToken cancellationToken = default) { Saves++; return Task.CompletedTask; }
}

