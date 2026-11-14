using marketgo_backend.Profiles.Domain.Model;
using marketgo_backend.Profiles.Domain.Model.Aggregates;
using marketgo_backend.Shared.Application.Model;
using marketgo_backend.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;

namespace marketgo_backend.Profiles.Interfaces.Rest.Transform;

public static class ProfilesActionResultAssembler
{
    public static IActionResult ToActionResult(
        ControllerBase controller, Result<Profile> result, ProblemDetailsFactory problems,
        Func<Profile, IActionResult> success)
    {
        if (result.IsSuccess) return success(result.Value!);
        var error = (ProfilesError)result.Error!;
        var status = error switch
        {
            ProfilesError.ProfileNotFound => StatusCodes.Status404NotFound,
            ProfilesError.ProfileAlreadyExists => StatusCodes.Status409Conflict,
            ProfilesError.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status400BadRequest
        };
        return problems.CreateProblemDetails(controller, status, error, result.Message);
    }
}
