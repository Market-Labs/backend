namespace marketgo_backend.Communication.Domain.Model.Queries;
public sealed record GetCommunicationQuery(Guid OrganizationId, bool IsAlert);
