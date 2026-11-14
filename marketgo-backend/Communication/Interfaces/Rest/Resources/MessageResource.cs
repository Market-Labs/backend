namespace marketgo_backend.Communication.Interfaces.Rest.Resources;
public sealed record MessageResource(Guid Id, string Sender, string Receiver, string Subject,
    string Body, bool Read, bool Starred, DateTimeOffset SentAt);
