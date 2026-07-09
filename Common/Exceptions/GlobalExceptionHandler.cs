using System.Text.Json;
using AuthService.Common.Dtos;
using AuthService.Common.ErrorCodes;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Common.Exceptions;


public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var traceId = httpContext.TraceIdentifier;

        var errorCode = MapErrorCode(exception);
        var errors = MapErrors(exception);

        if (exception is AppException)
        {
            logger.LogWarning(
                exception,
                "Application exception occurred. TraceId: {TraceId}, Code: {Code}",
                traceId,
                errorCode.Code);
        }
        else
        {
            logger.LogError(
                exception,
                "Unhandled exception occurred. TraceId: {TraceId}, Code: {Code}",
                traceId,
                errorCode.Code);
        }

        var response = ApiResponse<object>.Fail(
            errorCode,
            errors,
            traceId);

        httpContext.Response.StatusCode = errorCode.Code;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsync(
            JsonSerializer.Serialize(response, new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }),
            cancellationToken);

        return true;
    }

    private static ErrorCode MapErrorCode(Exception exception)
    {
        return exception switch
        {
            AppException appException => appException.ErrorCode,

            UnauthorizedAccessException => CommonErrors.Unauthorized,

            KeyNotFoundException => CommonErrors.NotFound,

            DbUpdateException => SystemErrors.DatabaseError,

            TimeoutException => SystemErrors.Timeout,

            TaskCanceledException => SystemErrors.Timeout,

            OperationCanceledException => SystemErrors.Timeout,

            HttpRequestException => SystemErrors.ExternalServiceError,

            JsonException => SystemErrors.InvalidJson,

            ArgumentException => CommonErrors.BadRequest,

            InvalidOperationException => CommonErrors.BadRequest,

            NotImplementedException => SystemErrors.InternalServerError,

            _ => SystemErrors.InternalServerError
        };
    }

    private static IReadOnlyList<string> MapErrors(Exception exception)
    {
        return exception switch
        {
            AppException appException => appException.Errors,
            _ => Array.Empty<string>()
        };
    }
}