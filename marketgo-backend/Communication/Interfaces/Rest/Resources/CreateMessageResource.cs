namespace marketgo_backend.Communication.Interfaces.Rest.Resources;
public sealed record CreateMessageResource(string Receiver, string Subject, string Body);
