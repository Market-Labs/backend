namespace marketgo_backend.Communication.Interfaces.Rest.Resources;
public sealed record CreateAlertResource(string Subject, string Body, string Severity);
