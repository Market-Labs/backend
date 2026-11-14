using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using marketgo_backend.Inventory.Application.CommandServices;
using marketgo_backend.Inventory.Application.QueryServices;
using marketgo_backend.Inventory.Domain.Model;
using marketgo_backend.Inventory.Domain.Model.Commands;
using marketgo_backend.Inventory.Interfaces.Rest.Resources;
using Microsoft.AspNetCore.Mvc;
namespace marketgo_backend.Inventory.Interfaces.Rest;
[Authorize, ApiController]
[Route("api/v1/minimarkets/{organizationId:guid}/waste")]
[Route("api/v1/suppliers/{organizationId:guid}/waste")]
[Tags("Inventory - Waste")]
public sealed class WasteController(IInventoryCommandService commands, IInventoryQueryService queries) : ControllerBase
{
    private bool Owns(Guid id) => ((User)HttpContext.Items["User"]!).OrganizationId == id;
    [HttpGet]
    public async Task<IActionResult> List(Guid organizationId, CancellationToken ct) =>
        !Owns(organizationId) ? Forbid() : Ok(await queries.ListWasteAsync(organizationId, ct));
    [HttpPost]
    public async Task<IActionResult> Create(Guid organizationId, CreateWasteResource resource, CancellationToken ct)
    {
        if (!Owns(organizationId)) return Forbid();
        var result = await commands.Handle(new CreateWasteCommand(organizationId, resource.InventoryItemId,
            resource.Quantity, resource.Reason), ct);
        return result.IsFailure ? result.Error is InventoryError.ItemNotFound ? NotFound() : BadRequest(new { error = result.Message }) :
            StatusCode(StatusCodes.Status201Created, result.Value);
    }
}
