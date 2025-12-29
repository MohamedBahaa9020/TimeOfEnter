using ErrorOr;
using Microsoft.AspNetCore.Mvc;
namespace TimeOfEnter.Common.Extensions;

public static class ErrorOrExtensions
{
    public static IActionResult Problem(
        this ControllerBase controller,
        List<Error> errors)
    {
        var firstError = errors[0];
        var statusCode = firstError.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };
        return controller.Problem(
            statusCode: statusCode,
            title: firstError.Description
        );
    }
}