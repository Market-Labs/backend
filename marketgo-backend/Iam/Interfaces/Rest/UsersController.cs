using marketgo_backend.Iam.Application.CommandServices;
using marketgo_backend.Iam.Application.QueryServices;
using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Iam.Domain.Model.Queries;
using marketgo_backend.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using marketgo_backend.Iam.Interfaces.Rest.Resources;
using marketgo_backend.Iam.Interfaces.Rest.Transform;
using marketgo_backend.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;

namespace marketgo_backend.Iam.Interfaces.Rest;

[Authorize]
[ApiController]
[Route("api/v1/users")]
[Tags("IAM - Users")]
public sealed class UsersController(IUserCommandService commands, IUserQueryService queries,
    ProblemDetailsFactory problems) : ControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthenticatedUserResource), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateUser(SignUpResource resource, CancellationToken cancellationToken)
    {
        var result = await commands.Handle(SignUpCommandFromResourceAssembler.ToCommandFromResource(resource), cancellationToken);
        return IamActionResultAssembler.ToActionResultFromSignUpResult(this, result, problems,
            value => StatusCode(StatusCodes.Status201Created,
                AuthenticatedUserResourceFromEntityAssembler.ToResourceFromEntity(value.user, value.token)));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UserResource), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserById(int id, CancellationToken cancellationToken)
    {
        var caller = (User)HttpContext.Items["User"]!;
        var user = await queries.Handle(new GetUserByIdQuery(id), cancellationToken);
        if (user is null || user.OrganizationId != caller.OrganizationId) return NotFound();
        return Ok(UserResourceFromEntityAssembler.ToResourceFromEntity(user));
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        var caller = (User)HttpContext.Items["User"]!;
        var users = await queries.Handle(new GetAllUsersQuery(caller.OrganizationId), cancellationToken);
        return Ok(users.Select(UserResourceFromEntityAssembler.ToResourceFromEntity));
    }
}
