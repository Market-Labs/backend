using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using marketgo_backend.Requisition.Application.CommandServices;
using marketgo_backend.Requisition.Application.QueryServices;
using marketgo_backend.Requisition.Domain.Model;
using marketgo_backend.Requisition.Domain.Model.Commands;
using marketgo_backend.Requisition.Domain.Model.Queries;
using marketgo_backend.Requisition.Interfaces.Rest.Resources;
using marketgo_backend.Requisition.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;
namespace marketgo_backend.Requisition.Interfaces.Rest;
[Authorize, ApiController]
[Route("api/v1/minimarkets/{organizationId:guid}/requisitions")]
[Route("api/v1/suppliers/{organizationId:guid}/requisitions")]
[Tags("Requisition")]
public sealed class RequisitionsController(ISupplyRequestCommandService commands, ISupplyRequestQueryService queries) : ControllerBase
{
    private User Caller => (User)HttpContext.Items["User"]!;
    private bool Owns(Guid id) => Caller.OrganizationId == id;
    [HttpGet]
    public async Task<IActionResult> List(Guid organizationId, CancellationToken ct) =>
        !Owns(organizationId) ? Forbid() :
        Ok((await queries.Handle(new GetSupplyRequestsQuery(organizationId, Caller.AccountType), ct))
            .Select(SupplyRequestResourceFromEntityAssembler.ToResourceFromEntity));
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid organizationId, Guid id, CancellationToken ct)
    {
        if (!Owns(organizationId)) return Forbid();
        var request = await queries.FindVisibleAsync(id, organizationId, Caller.AccountType, ct);
        return request is null ? NotFound() : Ok(SupplyRequestResourceFromEntityAssembler.ToResourceFromEntity(request));
    }
    [HttpPost]
    public async Task<IActionResult> Create(Guid organizationId, CreateSupplyRequestResource resource, CancellationToken ct)
    {
        if (!Owns(organizationId) || Caller.AccountType != "minimarket") return Forbid();
        var result = await commands.Handle(new CreateSupplyRequestCommand(organizationId,
            resource.SupplierId, Caller.Id.Value, Caller.FullName, resource.Supplier,
            resource.Reason, resource.Items), ct);
        return result.IsFailure ? BadRequest(new { error = result.Message }) :
            CreatedAtAction(nameof(Get), new { organizationId, id = result.Value!.Id },
                SupplyRequestResourceFromEntityAssembler.ToResourceFromEntity(result.Value!));
    }
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Review(Guid organizationId, Guid id, ReviewSupplyRequestResource resource, CancellationToken ct)
    {
        if (!Owns(organizationId) || Caller.AccountType != "supplier") return Forbid();
        var result = await commands.Handle(new ReviewSupplyRequestCommand(id, organizationId,
            resource.Status, resource.RejectionReason), ct);
        return result.IsFailure ? result.Error is RequisitionError.NotFound ? NotFound() :
            result.Error is RequisitionError.InvalidTransition ? Conflict(new { error = result.Message }) :
            BadRequest(new { error = result.Message }) :
            Ok(SupplyRequestResourceFromEntityAssembler.ToResourceFromEntity(result.Value!));
    }
}
