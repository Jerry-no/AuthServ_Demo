using System.Text.Json.Serialization;
using AuthService.Common.ErrorCodes;

namespace AuthService.Common.Dtos;

public sealed class ApiResponse<T>
{
    public bool Success { get; init; }

    public int Code { get; init; } = default!;

    public string Message { get; init; } = default!;

    public T? Data { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Errors { get; init; }

    public string? TraceId { get; init; }

    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;

    public static ApiResponse<T> Ok(
        T? data,
        string? message = null,
        string? traceId = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Code = CommonErrors.Success.Code,
            Message = message ?? CommonErrors.Success.Message,
            Data = data,
            TraceId = traceId
        };
    }

    public static ApiResponse<T> Fail(
        ErrorCode errorCode,
        object? errors = null,
        string? traceId = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Code = errorCode.Code,
            Message = errorCode.Message,
            Errors = errors,
            TraceId = traceId
        };
    }
}