namespace AuthService.Common.Dtos;

public sealed class ApiResult
{
    public object? Data { get; init; }

    public string? Message { get; init; }

    public int StatusCode { get; init; } = StatusCodes.Status200OK;
}