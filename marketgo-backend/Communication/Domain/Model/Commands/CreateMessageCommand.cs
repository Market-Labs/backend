namespace marketgo_backend.Communication.Domain.Model.Commands;
public sealed record CreateMessageCommand(Guid SenderOrganizationId, string SenderAccountType,
    Guid ReceiverOrganizationId, string Subject, string Body, Guid? IdempotencyKey);
