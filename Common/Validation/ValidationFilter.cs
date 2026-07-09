using AuthService.Common.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AuthService.Common.Validation;


public sealed class ValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid)
        {
            return;
        }

        var errors = context.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .SelectMany(x => x.Value!.Errors)
            .Select(x => x.ErrorMessage)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToArray();

        throw new ValidationAppException(errors);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}