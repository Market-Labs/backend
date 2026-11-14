using marketgo_backend.Conservation.Domain.Model.Aggregates;
using marketgo_backend.Conservation.Domain.Model.Commands;
using marketgo_backend.Shared.Application.Model;
namespace marketgo_backend.Conservation.Application.CommandServices;
public interface IConservationCommandService
{
    Task<Result<ConservationReading>> Handle(RecordConservationCommand command, CancellationToken cancellationToken);
}
