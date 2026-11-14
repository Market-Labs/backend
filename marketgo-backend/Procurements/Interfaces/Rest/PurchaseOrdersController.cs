using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using marketgo_backend.Procurements.Application.CommandServices;
using marketgo_backend.Procurements.Application.QueryServices;
using marketgo_backend.Procurements.Domain.Model;
using marketgo_backend.Procurements.Domain.Model.Commands;
using marketgo_backend.Procurements.Domain.Model.Queries;
using marketgo_backend.Procurements.Interfaces.Rest.Resources;
using marketgo_backend.Procurements.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;
namespace marketgo_backend.Procurements.Interfaces.Rest;
[Authorize, ApiController]
[Route("api/v1/minimarkets/{organizationId:guid}/purchase-orders")]
[Route("api/v1/suppliers/{organizationId:guid}/purchase-orders")]
[Tags("Procurements")]
public sealed class PurchaseOrdersController(IPurchaseOrderCommandService commands, IPurchaseOrderQueryService queries) : ControllerBase
{
    private User Caller => (User)HttpContext.Items["User"]!;
    private bool Owns(Guid id) => Caller.OrganizationId == id && Caller.Role == "owner" &&
        (Request.Path.StartsWithSegments("/api/v1/minimarkets", StringComparison.OrdinalIgnoreCase)
            ? Caller.AccountType == "minimarket" : Caller.AccountType == "supplier");
    [HttpGet]
    public async Task<IActionResult> List(Guid organizationId, CancellationToken ct) =>
        !Owns(organizationId) ? StatusCode(403) :
        Ok((await queries.Handle(new GetPurchaseOrdersQuery(organizationId, Caller.AccountType), ct))
            .Select(PurchaseOrderResourceFromEntityAssembler.ToResourceFromEntity));
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid organizationId, Guid id, CancellationToken ct)
    {
        if (!Owns(organizationId)) return StatusCode(403);
        var order = await queries.FindVisibleAsync(id, organizationId, Caller.AccountType, ct);
        return order is null ? NotFound() : Ok(PurchaseOrderResourceFromEntityAssembler.ToResourceFromEntity(order));
    }
    [HttpPost("/api/v1/suppliers/{organizationId:guid}/purchase-orders")]
    public async Task<IActionResult> Create(Guid organizationId, CreatePurchaseOrderResource resource, CancellationToken ct)
    {
        if (!Owns(organizationId) || Caller.AccountType != "supplier") return StatusCode(403);
        var result = await commands.Handle(new CreatePurchaseOrderCommand(organizationId, resource.SupplyRequestId,
            resource.Supplier, resource.Minimarket, resource.ShippingDate, resource.Observations, resource.Items), ct);
        return result.IsFailure ? result.Error is ProcurementsError.InvalidRequest ? Conflict(new { error = result.Message }) :
            BadRequest(new { error = result.Message }) :
            CreatedAtAction(nameof(Get), new { organizationId, id = result.Value!.Id },
                PurchaseOrderResourceFromEntityAssembler.ToResourceFromEntity(result.Value!));
    }
    [HttpPatch("/api/v1/minimarkets/{organizationId:guid}/purchase-orders/{id:guid}")]
    public async Task<IActionResult> Review(Guid organizationId, Guid id, ReviewPurchaseOrderResource resource, CancellationToken ct)
    {
        if (!Owns(organizationId) || Caller.AccountType != "minimarket" || Caller.Role != "owner") return StatusCode(403);
        var result = await commands.Handle(new ReviewPurchaseOrderCommand(id, organizationId, Caller.Id.Value,
            resource.Status, resource.RejectionReason, resource.ReceivedLines), ct);
        return result.IsFailure ? result.Error is ProcurementsError.NotFound ? NotFound() :
            result.Error is ProcurementsError.InvalidTransition ? Conflict(new { error = result.Message }) :
            BadRequest(new { error = result.Message }) :
            Ok(PurchaseOrderResourceFromEntityAssembler.ToResourceFromEntity(result.Value!));
    }
}
