namespace marketgo_backend.Communication.Domain.Model.Commands;
public sealed record CreateAlertCommand(Guid OrganizationId, string Subject, string Body, string Severity);
