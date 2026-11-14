using marketgo_backend.Analytics.Application.QueryServices;
using marketgo_backend.Analytics.Domain.Model.Queries;
using marketgo_backend.Analytics.Interfaces.Rest.Transform;
using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using Microsoft.AspNetCore.Mvc;
namespace marketgo_backend.Analytics.Interfaces.Rest;
[Authorize, ApiController]
[Route("api/v1/minimarkets/{organizationId:guid}/analytics")]
[Route("api/v1/suppliers/{organizationId:guid}/analytics")]
[Tags("Analytics")]
public sealed class AnalyticsController(IAnalyticsQueryService queries) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(Guid organizationId, CancellationToken ct)
    {
        var caller = (User)HttpContext.Items["User"]!;
        if (caller.OrganizationId != organizationId) return Forbid();
        var snapshot = await queries.Handle(new GetAnalyticsQuery(organizationId, caller.AccountType), ct);
        return Ok(AnalyticsResourceFromSnapshotAssembler.ToResource(snapshot));
    }
}
