namespace marketgo_backend.Communication.Interfaces.Rest.Resources;
public sealed record CreateMessageResource(Guid ReceiverOrganizationId, string Subject,
    string Body, Guid? IdempotencyKey = null);
