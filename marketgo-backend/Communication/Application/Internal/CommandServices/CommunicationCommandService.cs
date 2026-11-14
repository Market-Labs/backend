using marketgo_backend.Communication.Application.CommandServices;
using marketgo_backend.Communication.Domain.Model;
using marketgo_backend.Communication.Domain.Model.Aggregates;
using marketgo_backend.Communication.Domain.Model.Commands;
using marketgo_backend.Communication.Domain.Model.Errors;
using marketgo_backend.Communication.Domain.Repositories;
using marketgo_backend.Shared.Application.Model;
using marketgo_backend.Shared.Domain.Repositories;
namespace marketgo_backend.Communication.Application.Internal.CommandServices;
public sealed class CommunicationCommandService(ICommunicationRepository repository, IUnitOfWork unitOfWork)
    : ICommunicationCommandService
{
    public async Task<Result<Alert>> Handle(CreateAlertCommand command, CancellationToken ct)
    {
        try
        {
            var alert = new Alert(command.OrganizationId, command.Subject, command.Body, command.Severity);
            await repository.AddAlertAsync(alert, ct); await unitOfWork.CompleteAsync(ct);
            return Result<Alert>.Success(alert);
        }
        catch (ArgumentException) { return Result<Alert>.Failure(CommunicationErrors.InvalidData, CommunicationError.InvalidData); }
    }
    public async Task<Result<Message>> Handle(CreateMessageCommand command, CancellationToken ct)
    {
        try
        {
            var message = new Message(command.OrganizationId, command.Sender, command.Receiver,
                command.Subject, command.Body);
            await repository.AddMessageAsync(message, ct); await unitOfWork.CompleteAsync(ct);
            return Result<Message>.Success(message);
        }
        catch (ArgumentException) { return Result<Message>.Failure(CommunicationErrors.InvalidData, CommunicationError.InvalidData); }
    }
    public async Task<Result<object>> Handle(UpdateCommunicationCommand command, CancellationToken ct)
    {
        if (command.IsAlert)
        {
            var alert = await repository.FindAlertAsync(command.Id, command.OrganizationId, ct);
            if (alert is null) return Result<object>.Failure(CommunicationErrors.NotFound, CommunicationError.NotFound);
            alert.Update(command.Read, command.Starred); await unitOfWork.CompleteAsync(ct);
            return Result<object>.Success(alert);
        }
        var message = await repository.FindMessageAsync(command.Id, command.OrganizationId, ct);
        if (message is null) return Result<object>.Failure(CommunicationErrors.NotFound, CommunicationError.NotFound);
        message.Update(command.Read, command.Starred); await unitOfWork.CompleteAsync(ct);
        return Result<object>.Success(message);
    }
}
