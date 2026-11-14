using marketgo_backend.Communication.Domain.Model.Aggregates;
using marketgo_backend.Communication.Domain.Model.Commands;
using marketgo_backend.Shared.Application.Model;
namespace marketgo_backend.Communication.Application.CommandServices;
public interface ICommunicationCommandService
{
    Task<Result<Alert>> Handle(CreateAlertCommand command, CancellationToken cancellationToken);
    Task<Result<Message>> Handle(CreateMessageCommand command, CancellationToken cancellationToken);
    Task<Result<object>> Handle(UpdateCommunicationCommand command, CancellationToken cancellationToken);
}
