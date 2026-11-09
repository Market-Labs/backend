using System.Security.Claims;
using marketgo_backend.Analytics.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace marketgo_backend.Analytics.Interfaces.Rest;

[ApiController, Authorize]
[Route("api/v1/minimarkets/{organizationId:guid}/analytics")]
[Route("api/v1/suppliers/{organizationId:guid}/analytics")]
public sealed class AnalyticsController(IAnalyticsService analytics) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(Guid organizationId, CancellationToken ct)
    {
        if (!Guid.TryParse(User.FindFirstValue("organization_id"), out var current) || current != organizationId) return Forbid();
        return Ok(await analytics.GetAsync(organizationId, ct));
    }
}
