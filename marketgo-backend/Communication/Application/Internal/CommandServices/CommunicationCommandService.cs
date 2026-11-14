using marketgo_backend.Communication.Application.CommandServices;
using marketgo_backend.Communication.Domain.Model;
using marketgo_backend.Communication.Domain.Model.Aggregates;
using marketgo_backend.Communication.Domain.Model.Commands;
using marketgo_backend.Communication.Domain.Model.Errors;
using marketgo_backend.Communication.Domain.Repositories;
using marketgo_backend.Iam.Interfaces.Acl;
using marketgo_backend.Shared.Application.Model;
using marketgo_backend.Shared.Domain.Repositories;
using marketgo_backend.Suppliers.Interfaces.Acl;
using Microsoft.EntityFrameworkCore;
namespace marketgo_backend.Communication.Application.Internal.CommandServices;
public sealed class CommunicationCommandService(ICommunicationRepository repository, IUnitOfWork unitOfWork,
    IIamContextFacade organizations, ISuppliersContextFacade suppliers) : ICommunicationCommandService
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
        if (command.SenderOrganizationId == Guid.Empty || command.ReceiverOrganizationId == Guid.Empty ||
            command.SenderOrganizationId == command.ReceiverOrganizationId ||
            command.IdempotencyKey == Guid.Empty ||
            string.IsNullOrWhiteSpace(command.Subject) || string.IsNullOrWhiteSpace(command.Body) ||
            command.SenderAccountType is not ("minimarket" or "supplier"))
            return Result<Message>.Failure(CommunicationErrors.InvalidData, CommunicationError.InvalidData);
        var sender = await organizations.FindActiveOrganizationAsync(command.SenderOrganizationId, ct);
        var receiver = await organizations.FindActiveOrganizationAsync(command.ReceiverOrganizationId, ct);
        if (sender is null || sender.AccountType != command.SenderAccountType)
            return Result<Message>.Failure(CommunicationErrors.InvalidData, CommunicationError.InvalidData);
        if (receiver is null)
            return Result<Message>.Failure(CommunicationErrors.RecipientInvalid, CommunicationError.RecipientInvalid);
        if (sender.AccountType == receiver.AccountType)
            return Result<Message>.Failure(CommunicationErrors.InvalidData, CommunicationError.InvalidData);
        var miniId = sender.AccountType == "minimarket" ? sender.Id : receiver.Id;
        var supplierId = sender.AccountType == "supplier" ? sender.Id : receiver.Id;
        if (!await suppliers.IsLinkedActiveAsync(miniId, supplierId, ct))
            return Result<Message>.Failure(CommunicationErrors.NotLinked, CommunicationError.NotLinked);
        if (command.IdempotencyKey.HasValue)
        {
            var existing = await repository.FindByIdempotencyKeyAsync(sender.Id, command.IdempotencyKey.Value, ct);
            if (existing is not null)
                return existing.SameDelivery(receiver.Id, command.Subject, command.Body)
                    ? Result<Message>.Success(existing)
                    : Result<Message>.Failure(CommunicationErrors.DuplicateKey, CommunicationError.DuplicateKey);
        }
        try
        {
            var message = new Message(sender.Id, receiver.Id, sender.Name, receiver.Name,
                command.Subject, command.Body, command.IdempotencyKey);
            await repository.AddMessageAsync(message, ct);
            await unitOfWork.CompleteAsync(ct);
            return Result<Message>.Success(message);
        }
        catch (ArgumentException) { return Result<Message>.Failure(CommunicationErrors.InvalidData, CommunicationError.InvalidData); }
        catch (DbUpdateException) when (command.IdempotencyKey.HasValue)
        {
            var existing = await repository.FindByIdempotencyKeyAsync(sender.Id, command.IdempotencyKey.Value, ct);
            if (existing is null) throw;
            return existing.SameDelivery(receiver.Id, command.Subject, command.Body)
                ? Result<Message>.Success(existing)
                : Result<Message>.Failure(CommunicationErrors.DuplicateKey, CommunicationError.DuplicateKey);
        }
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
        if (message is null || (command.Read.HasValue && message.ReceiverOrganizationId != command.OrganizationId &&
            !(message.ReceiverOrganizationId is null && message.OrganizationId == command.OrganizationId)))
            return Result<object>.Failure(CommunicationErrors.NotFound, CommunicationError.NotFound);
        message.Update(command.Read, command.Starred); await unitOfWork.CompleteAsync(ct);
        return Result<object>.Success(message);
    }
}
