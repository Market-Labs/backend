using marketgo_backend.Iam.Domain.Model.Events;
using marketgo_backend.Profiles.Domain.Model.Aggregates;
using marketgo_backend.Profiles.Domain.Repositories;
using marketgo_backend.Shared.Application.Internal.EventHandlers;

namespace marketgo_backend.Profiles.Application.Internal.EventHandlers;

// The SaveChanges interceptor publishes the registration event before SQL is sent.
// Adding the profile to the same DbContext includes it in the original save.
public sealed class UserRegisteredEventHandler(IProfileRepository profiles) : IEventHandler<UserRegisteredEvent>
{
    public async Task Handle(UserRegisteredEvent domainEvent, CancellationToken cancellationToken)
    {
        var profile = new Profile(domainEvent.OrganizationId, domainEvent.Email,
            domainEvent.AccountType, domainEvent.OrganizationName, domainEvent.Email);
        await profiles.AddAsync(profile, cancellationToken);
    }
}
