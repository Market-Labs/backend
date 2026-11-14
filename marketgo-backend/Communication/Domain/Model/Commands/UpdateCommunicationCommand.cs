namespace marketgo_backend.Communication.Domain.Model.Commands;
public sealed record UpdateCommunicationCommand(Guid Id, Guid OrganizationId, bool IsAlert, bool? Read, bool? Starred);
