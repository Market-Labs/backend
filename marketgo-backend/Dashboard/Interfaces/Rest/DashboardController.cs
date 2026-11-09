using System.Security.Claims;
using marketgo_backend.Dashboard.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace marketgo_backend.Dashboard.Interfaces.Rest;

[ApiController, Authorize]
[Route("api/v1/minimarkets/{organizationId:guid}/dashboard")]
[Route("api/v1/suppliers/{organizationId:guid}/dashboard")]
public sealed class DashboardController(IDashboardService dashboard) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(Guid organizationId, CancellationToken ct)
    {
        if (!Guid.TryParse(User.FindFirstValue("organization_id"), out var current) || current != organizationId) return Forbid();
        return Ok(await dashboard.GetAsync(organizationId, ct));
    }
}
