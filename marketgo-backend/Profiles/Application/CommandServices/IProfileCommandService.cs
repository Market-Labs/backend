using marketgo_backend.Profiles.Domain.Model.Aggregates;
using marketgo_backend.Profiles.Domain.Model.Commands;
using marketgo_backend.Shared.Application.Model;

namespace marketgo_backend.Profiles.Application.CommandServices;

public interface IProfileCommandService
{
    Task<Result<Profile>> Handle(CreateProfileCommand command, CancellationToken cancellationToken);
    Task<Result<Profile>> Handle(UpdateProfileCommand command, CancellationToken cancellationToken);
}
