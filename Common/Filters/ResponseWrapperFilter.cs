using AuthService.Common.Dtos;
using AuthService.Common.ErrorCodes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AuthService.Common.Filters;

public sealed class ResponseWrapperFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(
        ResultExecutingContext context,
        ResultExecutionDelegate next)
    {
        if (ShouldSkipWrapping(context))
        {
            await next();
            return;
        }

        var traceId = context.HttpContext.TraceIdentifier;

        switch (context.Result)
        {
            case ObjectResult objectResult:
            {
                if (IsAlreadyWrapped(objectResult.Value))
                {
                    await next();
                    return;
                }

                var statusCode = objectResult.StatusCode ?? StatusCodes.Status200OK;

                if (statusCode >= 400)
                {
                    objectResult.Value = ApiResponse<object>.Fail(
                        new ErrorCode(statusCode, GetDefaultErrorMessage(statusCode)),
                        errors: ExtractErrors(objectResult.Value),
                        traceId: traceId);

                    objectResult.StatusCode = statusCode;
                    break;
                }

                if (objectResult.Value is ApiResult apiResult)
                {
                    objectResult.Value = ApiResponse<object>.Ok(
                        apiResult.Data,
                        message: apiResult.Message,
                        traceId: traceId);

                    objectResult.StatusCode = apiResult.StatusCode;
                    break;
                }

                objectResult.Value = ApiResponse<object>.Ok(
                    objectResult.Value,
                    traceId: traceId);

                break;
            }

            case EmptyResult:
            {
                context.Result = new ObjectResult(ApiResponse<object>.Ok(null, traceId: traceId))
                {
                    StatusCode = StatusCodes.Status200OK
                };

                break;
            }

            case OkResult:
            {
                context.Result = new OkObjectResult(ApiResponse<object>.Ok(null, traceId: traceId));
                break;
            }
        }

        await next();
    }

    private static bool ShouldSkipWrapping(ResultExecutingContext context)
    {
        return context.Result is FileResult
            or PhysicalFileResult
            or VirtualFileResult
            or FileStreamResult
            or ContentResult;
    }

    private static bool IsAlreadyWrapped(object? value)
    {
        if (value is null)
        {
            return false;
        }

        var type = value.GetType();

        return type.IsGenericType
               && type.GetGenericTypeDefinition() == typeof(ApiResponse<>);
    }
    
    private static string GetDefaultErrorMessage(int statusCode)
    {
        return statusCode switch
        {
            StatusCodes.Status400BadRequest => "Bad request",
            StatusCodes.Status401Unauthorized => "Unauthorized",
            StatusCodes.Status403Forbidden => "Forbidden",
            StatusCodes.Status404NotFound => "Not found",
            StatusCodes.Status409Conflict => "Conflict",
            StatusCodes.Status422UnprocessableEntity => "Validation failed",
            StatusCodes.Status500InternalServerError => "Internal server error",
            _ => "Request failed"
        };
    }
    private static object? ExtractErrors(object? value)
    {
        return value switch
        {
            ValidationProblemDetails validationProblem => validationProblem.Errors,

            ProblemDetails problem => new
            {
                problem.Title,
                problem.Detail,
                problem.Type,
                problem.Status
            },

            SerializableError serializableError => serializableError,

            _ => value
        };
    }
}