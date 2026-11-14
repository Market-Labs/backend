using marketgo_backend.Communication.Application.CommandServices;
using marketgo_backend.Communication.Application.QueryServices;
using marketgo_backend.Communication.Domain.Model;
using marketgo_backend.Communication.Domain.Model.Commands;
using marketgo_backend.Communication.Domain.Model.Queries;
using marketgo_backend.Communication.Interfaces.Rest.Resources;
using marketgo_backend.Communication.Interfaces.Rest.Transform;
using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using Microsoft.AspNetCore.Mvc;
namespace marketgo_backend.Communication.Interfaces.Rest;
[Authorize, ApiController]
[Route("api/v1/minimarkets/{organizationId:guid}/communication")]
[Route("api/v1/suppliers/{organizationId:guid}/communication")]
[Tags("Communication")]
public sealed class CommunicationController(ICommunicationCommandService commands, ICommunicationQueryService queries) : ControllerBase
{
    private User Caller => (User)HttpContext.Items["User"]!;
    private bool Authorized(Guid id) => Caller.OrganizationId == id && Caller.Role == "owner" &&
        (Request.Path.StartsWithSegments("/api/v1/minimarkets", StringComparison.OrdinalIgnoreCase)
            ? Caller.AccountType == "minimarket"
            : Caller.AccountType == "supplier");
    [HttpGet("alerts")]
    public async Task<IActionResult> Alerts(Guid organizationId, CancellationToken ct) =>
        !Authorized(organizationId) ? StatusCode(403) :
        Ok((await queries.Alerts(new GetCommunicationQuery(organizationId, true), ct))
            .Select(MessageResourceFromEntityAssembler.ToResourceFromEntity));
    [HttpPost("alerts")]
    public async Task<IActionResult> CreateAlert(Guid organizationId, CreateAlertResource resource, CancellationToken ct)
    {
        if (!Authorized(organizationId)) return StatusCode(403);
        var result = await commands.Handle(new CreateAlertCommand(organizationId,
            resource.Subject, resource.Body, resource.Severity), ct);
        return result.IsFailure ? BadRequest(new { error = result.Message }) :
            StatusCode(201, MessageResourceFromEntityAssembler.ToResourceFromEntity(result.Value!));
    }
    [HttpPatch("alerts/{id:guid}")]
    public Task<IActionResult> UpdateAlert(Guid organizationId, Guid id, UpdateCommunicationResource resource, CancellationToken ct) =>
        Update(organizationId, id, true, resource, ct);
    [HttpGet("messages")]
    public async Task<IActionResult> Messages(Guid organizationId, [FromQuery] string box = "all", CancellationToken ct = default)
    {
        if (!Authorized(organizationId)) return StatusCode(403);
        if (box is not ("all" or "inbox" or "sent")) return BadRequest(new { error = "box must be all, inbox or sent." });
        return Ok((await queries.Messages(new GetCommunicationQuery(organizationId, false, box), ct))
            .Select(MessageResourceFromEntityAssembler.ToResourceFromEntity));
    }
    [HttpPost("messages")]
    public async Task<IActionResult> CreateMessage(Guid organizationId, CreateMessageResource resource, CancellationToken ct)
    {
        if (!Authorized(organizationId)) return StatusCode(403);
        var result = await commands.Handle(new CreateMessageCommand(organizationId, Caller.AccountType,
            resource.ReceiverOrganizationId, resource.Subject, resource.Body, resource.IdempotencyKey), ct);
        if (result.IsFailure) return result.Error switch
        {
            CommunicationError.RecipientInvalid => NotFound(new { error = result.Message }),
            CommunicationError.DuplicateKey => Conflict(new { error = result.Message }),
            _ => BadRequest(new { error = result.Message })
        };
        return StatusCode(201,
            MessageResourceFromEntityAssembler.ToResourceFromEntity(result.Value!));
    }
    [HttpPatch("messages/{id:guid}")]
    public Task<IActionResult> UpdateMessage(Guid organizationId, Guid id, UpdateCommunicationResource resource, CancellationToken ct) =>
        Update(organizationId, id, false, resource, ct);
    private async Task<IActionResult> Update(Guid organizationId, Guid id, bool alert, UpdateCommunicationResource resource, CancellationToken ct)
    {
        if (!Authorized(organizationId)) return StatusCode(403);
        var result = await commands.Handle(new UpdateCommunicationCommand(id, organizationId, alert,
            resource.Read, resource.Starred), ct);
        if (result.IsFailure) return result.Error is CommunicationError.NotFound ? NotFound() : BadRequest();
        return result.Value is Communication.Domain.Model.Aggregates.Alert a ?
            Ok(MessageResourceFromEntityAssembler.ToResourceFromEntity(a)) :
            Ok(MessageResourceFromEntityAssembler.ToResourceFromEntity((Communication.Domain.Model.Aggregates.Message)result.Value!));
    }
}
