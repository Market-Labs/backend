using marketgo_backend.Iam.Domain.Model;
using marketgo_backend.Iam.Domain.Model.Aggregates;
using marketgo_backend.Shared.Application.Model;
using marketgo_backend.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;

namespace marketgo_backend.Iam.Interfaces.Rest.Transform;

public static class IamActionResultAssembler
{
    public static IActionResult ToActionResultFromSignInResult(
        ControllerBase controller, Result<(User user, string token)> result,
        ProblemDetailsFactory problems, Func<(User user, string token), IActionResult> success) =>
        ToActionResult(controller, result, problems, success);

    public static IActionResult ToActionResultFromSignUpResult(
        ControllerBase controller, Result<(User user, string token)> result,
        ProblemDetailsFactory problems, Func<(User user, string token), IActionResult> success) =>
        ToActionResult(controller, result, problems, success);

    private static IActionResult ToActionResult(
        ControllerBase controller, Result<(User user, string token)> result,
        ProblemDetailsFactory problems, Func<(User user, string token), IActionResult> success)
    {
        if (result.IsSuccess) return success(result.Value!);
        var error = (IamError)result.Error!;
        var status = error switch
        {
            IamError.InvalidCredentials => StatusCodes.Status401Unauthorized,
            IamError.EmailAlreadyTaken => StatusCodes.Status409Conflict,
            IamError.UserNotFound => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status400BadRequest
        };
        return problems.CreateProblemDetails(controller, status, error, result.Message);
    }
}
