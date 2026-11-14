using marketgo_backend.Communication.Domain.Model.Aggregates;
using marketgo_backend.Communication.Interfaces.Rest.Resources;
namespace marketgo_backend.Communication.Interfaces.Rest.Transform;
public static class MessageResourceFromEntityAssembler
{
    public static MessageResource ToResourceFromEntity(Alert alert) =>
        new(alert.Id, alert.Sender, string.Empty, alert.Subject, alert.Body,
            alert.Read, alert.Starred, alert.SentAt);
    public static MessageResource ToResourceFromEntity(Message message) =>
        new(message.Id, message.Sender, message.Receiver, message.Subject, message.Body,
            message.Read, message.Starred, message.SentAt);
}
