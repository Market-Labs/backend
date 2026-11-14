using marketgo_backend.Dashboard.Application.QueryServices;
using marketgo_backend.Dashboard.Domain.Model.Queries;
using marketgo_backend.Dashboard.Interfaces.Rest.Transform;
using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using Microsoft.AspNetCore.Mvc;
namespace marketgo_backend.Dashboard.Interfaces.Rest;
[Authorize, ApiController]
[Route("api/v1/minimarkets/{organizationId:guid}/dashboard")]
[Route("api/v1/suppliers/{organizationId:guid}/dashboard")]
[Tags("Dashboard")]
public sealed class DashboardController(IDashboardQueryService queries) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(Guid organizationId, CancellationToken ct)
    {
        var caller = (User)HttpContext.Items["User"]!;
        if (caller.OrganizationId != organizationId) return Forbid();
        var snapshot = await queries.Handle(new GetDashboardQuery(organizationId, caller.AccountType), ct);
        return Ok(DashboardResourceFromSnapshotAssembler.ToResource(snapshot));
    }
}
