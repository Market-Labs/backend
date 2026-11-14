using marketgo_backend.Conservation.Application.CommandServices;
using marketgo_backend.Conservation.Application.QueryServices;
using marketgo_backend.Conservation.Domain.Model.Commands;
using marketgo_backend.Conservation.Domain.Model.Queries;
using marketgo_backend.Conservation.Interfaces.Rest.Resources;
using marketgo_backend.Conservation.Interfaces.Rest.Transform;
using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using Microsoft.AspNetCore.Mvc;
namespace marketgo_backend.Conservation.Interfaces.Rest;
[Authorize, ApiController]
[Route("api/v1/minimarkets/{organizationId:guid}/conservation/monitoring")]
[Route("api/v1/suppliers/{organizationId:guid}/conservation/monitoring")]
[Tags("Conservation")]
public sealed class ConservationController(IConservationCommandService commands, IConservationQueryService queries) : ControllerBase
{
    private bool Owns(Guid id) => ((User)HttpContext.Items["User"]!).OrganizationId == id;
    [HttpGet]
    public async Task<IActionResult> List(Guid organizationId, CancellationToken ct) =>
        !Owns(organizationId) ? Forbid() :
        Ok((await queries.Handle(new GetConservationReadingsQuery(organizationId), ct))
            .Select(ConservationResourceFromEntityAssembler.ToResourceFromEntity));
    [HttpPost]
    public async Task<IActionResult> Record(Guid organizationId, RecordConservationResource resource, CancellationToken ct)
    {
        if (!Owns(organizationId)) return Forbid();
        var result = await commands.Handle(new RecordConservationCommand(organizationId, resource.Zone,
            resource.ProductName, resource.Temperature, resource.Humidity, resource.RecordedAt), ct);
        return result.IsFailure ? BadRequest(new { error = result.Message }) :
            StatusCode(StatusCodes.Status201Created, ConservationResourceFromEntityAssembler.ToResourceFromEntity(result.Value!));
    }
}
