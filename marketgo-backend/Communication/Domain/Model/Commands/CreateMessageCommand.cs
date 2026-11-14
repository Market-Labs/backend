namespace marketgo_backend.Communication.Domain.Model.Commands;
public sealed record CreateMessageCommand(Guid OrganizationId, string Sender, string Receiver, string Subject, string Body);
