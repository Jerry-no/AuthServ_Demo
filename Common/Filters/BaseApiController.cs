using AuthService.Common.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Common.Filters;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected IActionResult Success(
        object? data = null,
        string? message = null,
        int statusCode = StatusCodes.Status200OK)
    {
        return StatusCode(statusCode, new ApiResult()
        {
            Data = data,
            Message = message,
            StatusCode = statusCode
        });
    }
}