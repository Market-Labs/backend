using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using marketgo_backend.Inventory.Application.CommandServices;
using marketgo_backend.Inventory.Application.QueryServices;
using marketgo_backend.Inventory.Domain.Model;
using marketgo_backend.Inventory.Domain.Model.Commands;
using marketgo_backend.Inventory.Domain.Model.Queries;
using marketgo_backend.Inventory.Interfaces.Rest.Resources;
using marketgo_backend.Inventory.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;
namespace marketgo_backend.Inventory.Interfaces.Rest;
[Authorize, ApiController]
[Route("api/v1/minimarkets/{organizationId:guid}/inventory")]
[Route("api/v1/suppliers/{organizationId:guid}/inventory")]
[Tags("Inventory")]
public sealed class InventoryController(IInventoryCommandService commands, IInventoryQueryService queries) : ControllerBase
{
    private bool Owns(Guid id) => ((User)HttpContext.Items["User"]!).OrganizationId == id;
    [HttpGet]
    public async Task<IActionResult> List(Guid organizationId, CancellationToken ct) =>
        !Owns(organizationId) ? Forbid() :
        Ok((await queries.Handle(new GetInventoryQuery(organizationId), ct))
            .Select(InventoryItemResourceFromEntityAssembler.ToResourceFromEntity));
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid organizationId, Guid id, CancellationToken ct)
    {
        if (!Owns(organizationId)) return Forbid();
        var item = await queries.FindAsync(id, organizationId, ct);
        return item is null ? NotFound() : Ok(InventoryItemResourceFromEntityAssembler.ToResourceFromEntity(item));
    }
    [HttpGet("search")]
    public async Task<IActionResult> Search(Guid organizationId, [FromQuery] string? query, CancellationToken ct) =>
        !Owns(organizationId) ? Forbid() :
        Ok((await queries.Handle(new GetInventoryQuery(organizationId, query), ct))
            .Select(InventoryItemResourceFromEntityAssembler.ToResourceFromEntity));
    [HttpGet("lots")]
    [HttpGet("/api/v1/minimarkets/{organizationId:guid}/lots")]
    [HttpGet("/api/v1/suppliers/{organizationId:guid}/lots")]
    public Task<IActionResult> Lots(Guid organizationId, CancellationToken ct) => List(organizationId, ct);
    [HttpGet("expirations")]
    [HttpGet("/api/v1/minimarkets/{organizationId:guid}/expirations")]
    [HttpGet("/api/v1/suppliers/{organizationId:guid}/expirations")]
    public async Task<IActionResult> Expirations(Guid organizationId, [FromQuery] int days = 7, CancellationToken ct = default)
    {
        if (!Owns(organizationId)) return Forbid();
        if (days is < 0 or > 365) return BadRequest(new { error = "Days must be between 0 and 365." });
        var items = await queries.Handle(new GetInventoryQuery(organizationId, ExpiringWithinDays: days), ct);
        return Ok(items.Select(InventoryItemResourceFromEntityAssembler.ToResourceFromEntity));
    }
    [HttpPost]
    public async Task<IActionResult> Create(Guid organizationId, CreateInventoryItemResource resource, CancellationToken ct)
    {
        if (!Owns(organizationId)) return Forbid();
        var result = await commands.Handle(new CreateInventoryItemCommand(organizationId, resource.ProductId,
            resource.Stock, resource.MinimumStock, resource.LotCode, resource.ExpirationDate), ct);
        return result.IsFailure ? Error(result.Error) :
            CreatedAtAction(nameof(Get), new { organizationId, id = result.Value!.Id },
                InventoryItemResourceFromEntityAssembler.ToResourceFromEntity(result.Value!));
    }
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid organizationId, Guid id, UpdateInventoryItemResource resource, CancellationToken ct)
    {
        if (!Owns(organizationId)) return Forbid();
        var result = await commands.Handle(new UpdateInventoryItemCommand(id, organizationId,
            resource.Stock, resource.MinimumStock), ct);
        return result.IsFailure ? Error(result.Error) : Ok(InventoryItemResourceFromEntityAssembler.ToResourceFromEntity(result.Value!));
    }
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid organizationId, Guid id, CancellationToken ct)
    {
        if (!Owns(organizationId)) return Forbid();
        var result = await commands.Delete(id, organizationId, ct);
        return result.IsFailure ? Error(result.Error) : NoContent();
    }
    private IActionResult Error(Enum? error) => error switch
    {
        InventoryError.ItemNotFound => NotFound(),
        InventoryError.ProductUnavailable => BadRequest(new { error = "Product is unavailable." }),
        _ => BadRequest(new { error = "Invalid inventory data." })
    };
}
