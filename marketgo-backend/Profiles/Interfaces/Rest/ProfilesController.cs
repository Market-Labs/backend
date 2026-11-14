using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using marketgo_backend.Profiles.Application.CommandServices;
using marketgo_backend.Profiles.Application.QueryServices;
using marketgo_backend.Profiles.Domain.Model.Queries;
using marketgo_backend.Profiles.Interfaces.Rest.Resources;
using marketgo_backend.Profiles.Interfaces.Rest.Transform;
using marketgo_backend.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;

namespace marketgo_backend.Profiles.Interfaces.Rest;

[Authorize]
[ApiController]
[Route("api/v1/profiles")]
[Tags("Profiles")]
public sealed class ProfilesController(IProfileCommandService commands, IProfileQueryService queries,
    ProblemDetailsFactory problems) : ControllerBase
{
    private User Caller => (User)HttpContext.Items["User"]!;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PublicProfileResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllProfiles(CancellationToken cancellationToken)
    {
        var profiles = await queries.Handle(new GetAllProfilesQuery(), cancellationToken);
        return Ok(profiles.Select(ProfileResourceFromEntityAssembler.ToPublicResourceFromEntity));
    }

    [HttpGet("{profileId:int}")]
    [ProducesResponseType(typeof(ProfileResource), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProfileById(int profileId, CancellationToken cancellationToken)
    {
        var profile = await queries.Handle(new GetProfileByIdQuery(profileId), cancellationToken);
        if (profile is null) return NotFound();
        return profile.OrganizationId == Caller.OrganizationId
            ? Ok(ProfileResourceFromEntityAssembler.ToResourceFromEntity(profile, Caller))
            : Ok(ProfileResourceFromEntityAssembler.ToPublicResourceFromEntity(profile));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProfileResource), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateProfile(CreateProfileResource resource, CancellationToken cancellationToken)
    {
        var caller = Caller;
        var command = CreateProfileCommandFromResourceAssembler.ToCommandFromResource(resource, caller);
        var result = await commands.Handle(command, cancellationToken);
        return ProfilesActionResultAssembler.ToActionResult(this, result, problems,
            profile => CreatedAtAction(nameof(GetProfileById), new { profileId = profile.Id.Value },
                ProfileResourceFromEntityAssembler.ToResourceFromEntity(profile, caller)));
    }

    [HttpPatch("{profileId:int}")]
    [ProducesResponseType(typeof(ProfileResource), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateProfile(int profileId, UpdateProfileResource resource,
        CancellationToken cancellationToken)
    {
        var caller = Caller;
        var command = UpdateProfileCommandFromResourceAssembler.ToCommandFromResource(profileId, resource, caller);
        var result = await commands.Handle(command, cancellationToken);
        return ProfilesActionResultAssembler.ToActionResult(this, result, problems,
            profile => Ok(ProfileResourceFromEntityAssembler.ToResourceFromEntity(profile, caller)));
    }
}
