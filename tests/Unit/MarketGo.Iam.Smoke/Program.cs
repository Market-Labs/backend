using marketgo_backend.Iam.Application.Internal.CommandServices;
using marketgo_backend.Iam.Domain.Model;
using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Iam.Domain.Model.Commands;
using marketgo_backend.Iam.Domain.Model.ValueObjects;
using marketgo_backend.Iam.Domain.Repositories;
using marketgo_backend.Iam.Infrastructure.Hashing.BCrypt.Services;
using marketgo_backend.Iam.Infrastructure.Tokens.Jwt.Configuration;
using marketgo_backend.Iam.Infrastructure.Tokens.Jwt.Services;
using marketgo_backend.Iam.Interfaces.Rest.Transform;
using marketgo_backend.Shared.Domain.Repositories;
using marketgo_backend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var users = new MemoryUsers();
var work = new MemoryWork();
var hashing = new HashingService();
var tokens = new TokenService(Options.Create(new TokenSettings {
    Secret = new string('k', 48), Issuer = "marketgo-backend", Audience = "marketgo-frontend"
}));
var service = new UserCommandService(users, tokens, hashing, work, TimeProvider.System);
var invalid = await service.Handle(new SignUpCommand("Ana", "ana@example.com", "password123", "Tienda", "minimarket", false), default);
Check(invalid.IsFailure && invalid.Error is IamError.InvalidRegistration, "Terms required");
var registered = await service.Handle(new SignUpCommand("Ana", "ana@example.com", "password123", "Tienda", "minimarket", true), default);
Check(registered.IsSuccess, "Registration succeeds");
var (user, token) = registered.Value;
Check(user.Id.Value == 1 && user.OrganizationId != Guid.Empty, "User and organization ids");
Check(user.PasswordHash != "password123" && hashing.VerifyPassword("password123", user.PasswordHash), "BCrypt password hash");
Check(await tokens.ValidateToken(token) == 1, "JWT validates");
Check(UserResourceFromEntityAssembler.ToResourceFromEntity(user).MinimarketId == user.OrganizationId, "MarketGo user resource");
var duplicate = await service.Handle(new SignUpCommand("Otro", "ANA@example.com", "password123", "Tienda", "minimarket", true), default);
Check(duplicate.IsFailure && duplicate.Error is IamError.EmailAlreadyTaken, "Unique email ignores case");
var wrong = await service.Handle(new SignInCommand("ana@example.com", "wrong"), default);
Check(wrong.IsFailure && wrong.Error is IamError.InvalidCredentials, "Wrong password rejected");
var login = await service.Handle(new SignInCommand("ANA@example.com", "password123"), default);
Check(login.IsSuccess && await tokens.ValidateToken(login.Value.token) == 1, "Sign in issues JWT");
Check(work.Saves == 2, "Registration and login saved");
using var db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
    .UseMySQL("server=localhost;database=marketgo;user=test;password=test").Options);
Check(db.Model.FindEntityType(typeof(User))?.GetTableName() == "users", "EF user mapping");
Console.WriteLine("IAM smoke passed: registration, JWT, BCrypt, validation, EF mapping.");

static void Check(bool value, string message) { if (!value) throw new Exception(message); }

sealed class MemoryWork : IUnitOfWork
{
    public int Saves { get; private set; }
    public Task CompleteAsync(CancellationToken token = default) { Saves++; return Task.CompletedTask; }
}
sealed class MemoryUsers : IUserRepository
{
    private readonly List<User> _users = [];
    public Task AddAsync(User entity, CancellationToken token = default)
    {
        typeof(User).GetProperty(nameof(User.Id))!.SetValue(entity, new UserId(_users.Count + 1));
        _users.Add(entity);
        return Task.CompletedTask;
    }
    public Task<User?> FindByIdAsync(int id, CancellationToken token = default) =>
        Task.FromResult(_users.SingleOrDefault(u => u.Id.Value == id));
    public Task<User?> FindByIdAsNoTrackingAsync(int id, CancellationToken token = default) => FindByIdAsync(id, token);
    public Task<User?> FindByEmailAsync(string email, CancellationToken token = default) =>
        Task.FromResult(_users.SingleOrDefault(u => u.NormalizedEmail == email));
    public Task<bool> ExistsByEmailAsync(string email, CancellationToken token = default) =>
        Task.FromResult(_users.Any(u => u.NormalizedEmail == email));
    public Task<IReadOnlyList<User>> ListByOrganizationAsync(Guid id, CancellationToken token = default) =>
        Task.FromResult<IReadOnlyList<User>>(_users.Where(u => u.OrganizationId == id).ToList());
    public Task<IEnumerable<User>> ListAsync(CancellationToken token = default) =>
        Task.FromResult<IEnumerable<User>>(_users);
    public Task<IEnumerable<User>> ListAsNoTrackingAsync(CancellationToken token = default) => ListAsync(token);
    public void Update(User entity) { }
    public void Remove(User entity) => _users.Remove(entity);
}
