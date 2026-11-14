using marketgo_backend.Conservation.Application.CommandServices;
using marketgo_backend.Conservation.Domain.Model;
using marketgo_backend.Conservation.Domain.Model.Aggregates;
using marketgo_backend.Conservation.Domain.Model.Commands;
using marketgo_backend.Conservation.Domain.Model.Errors;
using marketgo_backend.Conservation.Domain.Repositories;
using marketgo_backend.Shared.Application.Model;
using marketgo_backend.Shared.Domain.Repositories;
namespace marketgo_backend.Conservation.Application.Internal.CommandServices;
public sealed class ConservationCommandService(IConservationRepository readings, IUnitOfWork unitOfWork)
    : IConservationCommandService
{
    public async Task<Result<ConservationReading>> Handle(RecordConservationCommand command, CancellationToken ct)
    {
        try
        {
            var reading = new ConservationReading(command.OrganizationId, command.Zone,
                command.ProductName, command.Temperature, command.Humidity, command.RecordedAt);
            await readings.AddAsync(reading, ct);
            await unitOfWork.CompleteAsync(ct);
            return Result<ConservationReading>.Success(reading);
        }
        catch (ArgumentException) { return Result<ConservationReading>.Failure(ConservationErrors.InvalidReading, ConservationError.InvalidReading); }
    }
}
