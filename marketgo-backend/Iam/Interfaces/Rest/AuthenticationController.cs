using marketgo_backend.Iam.Application.CommandServices;
using marketgo_backend.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using marketgo_backend.Iam.Interfaces.Rest.Resources;
using marketgo_backend.Iam.Interfaces.Rest.Transform;
using marketgo_backend.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;

namespace marketgo_backend.Iam.Interfaces.Rest;

[Authorize]
[ApiController]
[Route("api/v1/auth")]
[Tags("IAM - Authentication")]
public sealed class AuthenticationController(IUserCommandService commands, ProblemDetailsFactory problems) : ControllerBase
{
    [HttpPost("sign-in")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthenticatedUserResource), StatusCodes.Status200OK)]
    public async Task<IActionResult> SignIn(SignInResource resource, CancellationToken cancellationToken)
    {
        var result = await commands.Handle(SignInCommandFromResourceAssembler.ToCommandFromResource(resource), cancellationToken);
        return IamActionResultAssembler.ToActionResultFromSignInResult(this, result, problems,
            value => Ok(AuthenticatedUserResourceFromEntityAssembler.ToResourceFromEntity(value.user, value.token)));
    }

    [HttpPost("sign-up")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthenticatedUserResource), StatusCodes.Status201Created)]
    public async Task<IActionResult> SignUp(SignUpResource resource, CancellationToken cancellationToken)
    {
        var result = await commands.Handle(SignUpCommandFromResourceAssembler.ToCommandFromResource(resource), cancellationToken);
        return IamActionResultAssembler.ToActionResultFromSignUpResult(this, result, problems,
            value => StatusCode(StatusCodes.Status201Created,
                AuthenticatedUserResourceFromEntityAssembler.ToResourceFromEntity(value.user, value.token)));
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(UserResource), StatusCodes.Status200OK)]
    public IActionResult Me()
    {
        var user = (Iam.Domain.Model.Aggregates.User)HttpContext.Items["User"]!;
        return Ok(UserResourceFromEntityAssembler.ToResourceFromEntity(user));
    }
}
