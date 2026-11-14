using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Iam.Domain.Model.Events;
using marketgo_backend.Iam.Domain.Model.ValueObjects;
using marketgo_backend.Profiles.Application.Internal.CommandServices;
using marketgo_backend.Profiles.Application.Internal.EventHandlers;
using marketgo_backend.Profiles.Domain.Model;
using marketgo_backend.Profiles.Domain.Model.Aggregates;
using marketgo_backend.Profiles.Domain.Model.Commands;
using marketgo_backend.Profiles.Domain.Model.ValueObjects;
using marketgo_backend.Profiles.Domain.Repositories;
using marketgo_backend.Profiles.Interfaces.Rest.Transform;
using marketgo_backend.Shared.Domain.Repositories;
using marketgo_backend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore;

var profiles = new MemoryProfiles();
var work = new MemoryWork();
var commands = new ProfileCommandService(profiles, work);
var registered = new UserRegisteredEvent(Guid.NewGuid(), "owner@example.com", "Minimarket Sur", "minimarket");
await new UserRegisteredEventHandler(profiles).Handle(registered, default);
Check(profiles.Items.Count == 1, "Registration creates profile in same repository");
var profile = profiles.Items.Single();
Check(profile.OrganizationId == registered.OrganizationId && profile.BusinessName == "Minimarket Sur", "Registration fields");
var caller = new User("Ana", "owner@example.com", "hash", "Minimarket Sur", "minimarket");
typeof(User).GetProperty(nameof(User.Id))!.SetValue(caller, new UserId(5));
typeof(User).GetProperty(nameof(User.OrganizationId))!.SetValue(caller, registered.OrganizationId);
Check(ProfileResourceFromEntityAssembler.ToResourceFromEntity(profile, caller).UserId == 5, "Own profile links to user");
var forbidden = await commands.Handle(new UpdateProfileCommand(profile.Id.Value, Guid.NewGuid(),
    "Otro", null, null, null, null, null, null), default);
Check(forbidden.IsFailure && forbidden.Error is ProfilesError.Forbidden, "Other organization cannot edit");
var updated = await commands.Handle(new UpdateProfileCommand(profile.Id.Value, registered.OrganizationId,
    "Tienda Actualizada", "contacto@example.com", "999111222", "Av. Lima 1", "Surco", null, null), default);
Check(updated.IsSuccess && profile.BusinessName == "Tienda Actualizada" && profile.District == "Surco", "Owner can edit");
var duplicate = await commands.Handle(new CreateProfileCommand(registered.OrganizationId, registered.Email,
    "minimarket", "Duplicada", "duplicate@example.com", null, null, null, null, null), default);
Check(duplicate.IsFailure && duplicate.Error is ProfilesError.ProfileAlreadyExists, "One profile per organization");
var invalid = await commands.Handle(new CreateProfileCommand(Guid.NewGuid(), registered.Email,
    "supplier", "Proveedor", "bad-email", null, null, null, null, null), default);
Check(invalid.IsFailure && invalid.Error is ProfilesError.InvalidProfileData, "Invalid email rejected");
using var db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
    .UseMySQL("server=localhost;database=marketgo;user=test;password=test").Options);
var model = db.Model.FindEntityType(typeof(Profile))!;
Check(model.GetTableName() == "profiles", "EF profile table");
Check(model.GetIndexes().Any(i => i.IsUnique && i.Properties.Any(p => p.Name == nameof(Profile.OrganizationId))),
    "Unique organization index");
Console.WriteLine("Profiles smoke passed: registration event, ownership, updates, validation, EF mapping.");

static void Check(bool value, string message) { if (!value) throw new Exception(message); }
sealed class MemoryWork : IUnitOfWork
{
    public int Saves { get; private set; }
    public Task CompleteAsync(CancellationToken token = default) { Saves++; return Task.CompletedTask; }
}
sealed class MemoryProfiles : IProfileRepository
{
    public List<Profile> Items { get; } = [];
    public Task AddAsync(Profile entity, CancellationToken token = default)
    {
        typeof(Profile).GetProperty(nameof(Profile.Id))!.SetValue(entity, new ProfileId(Items.Count + 1));
        Items.Add(entity);
        return Task.CompletedTask;
    }
    public Task<Profile?> FindByIdAsync(int id, CancellationToken token = default) =>
        Task.FromResult(Items.SingleOrDefault(p => p.Id.Value == id));
    public Task<Profile?> FindByIdAsNoTrackingAsync(int id, CancellationToken token = default) => FindByIdAsync(id, token);
    public Task<Profile?> FindByOrganizationAsync(Guid id, CancellationToken token = default) =>
        Task.FromResult(Items.SingleOrDefault(p => p.OrganizationId == id));
    public Task<Profile?> FindProfileByEmailAsync(string email, CancellationToken token = default) =>
        Task.FromResult(Items.SingleOrDefault(p => p.Email == email));
    public Task<Profile?> FindProfileByEmailAsNoTrackingAsync(string email, CancellationToken token = default) =>
        FindProfileByEmailAsync(email, token);
    public Task<IReadOnlyList<Profile>> ListPublicAsync(CancellationToken token = default) =>
        Task.FromResult<IReadOnlyList<Profile>>(Items);
    public Task<IEnumerable<Profile>> ListAsync(CancellationToken token = default) =>
        Task.FromResult<IEnumerable<Profile>>(Items);
    public Task<IEnumerable<Profile>> ListAsNoTrackingAsync(CancellationToken token = default) => ListAsync(token);
    public void Update(Profile profile) { }
    public void Remove(Profile profile) => Items.Remove(profile);
}
