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
[Route("api/v1/minimarkets/{organizationId:guid}/donations")]
[Tags("Inventory - Donations")]
public sealed class DonationsController(IInventoryCommandService commands, IInventoryQueryService queries) : ControllerBase
{
    private bool Owns(Guid id) => ((User)HttpContext.Items["User"]!) is { } caller &&
        caller.OrganizationId == id && caller.AccountType == "minimarket" && caller.Role == "owner";
    [HttpGet]
    public async Task<IActionResult> List(Guid organizationId, CancellationToken ct) =>
        !Owns(organizationId) ? StatusCode(403) : Ok(await queries.ListDonationsAsync(organizationId, ct));
    [HttpPost]
    public async Task<IActionResult> Create(Guid organizationId, CreateDonationResource resource, CancellationToken ct)
    {
        if (!Owns(organizationId)) return StatusCode(403);
        var result = await commands.Handle(new CreateDonationCommand(organizationId, resource.InventoryItemId,
            resource.Quantity, resource.Recipient), ct);
        return result.IsFailure ? result.Error is InventoryError.ItemNotFound ? NotFound() : BadRequest(new { error = result.Message }) :
            StatusCode(StatusCodes.Status201Created, result.Value);
    }
}
