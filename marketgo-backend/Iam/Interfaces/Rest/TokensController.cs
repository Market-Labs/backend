using marketgo_backend.Iam.Application.CommandServices;
using marketgo_backend.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using marketgo_backend.Iam.Interfaces.Rest.Resources;
using marketgo_backend.Iam.Interfaces.Rest.Transform;
using marketgo_backend.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;

namespace marketgo_backend.Iam.Interfaces.Rest;

[ApiController]
[Route("api/v1/tokens")]
[Tags("IAM - Tokens")]
public sealed class TokensController(IUserCommandService commands, ProblemDetailsFactory problems) : ControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthenticatedUserResource), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateToken(SignInResource resource, CancellationToken cancellationToken)
    {
        var result = await commands.Handle(SignInCommandFromResourceAssembler.ToCommandFromResource(resource), cancellationToken);
        return IamActionResultAssembler.ToActionResultFromSignInResult(this, result, problems,
            value => Ok(AuthenticatedUserResourceFromEntityAssembler.ToResourceFromEntity(value.user, value.token)));
    }
}
