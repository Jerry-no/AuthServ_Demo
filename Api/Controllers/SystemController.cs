using AuthService.Api.System;
using AuthService.Common.Filters;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers;


[Route("api/system")]
public sealed class SystemController(
    ILogger<SystemController> logger) : BaseApiController
{
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        logger.LogInformation("Ping endpoint called");

        return Success(
            data:new
        {
            message = "pong"
        },
            message: "Ping success!"
        
        );
    }
    
    [HttpPost("ping")]
    public IActionResult PingWithBody([FromBody] PingRequest request)
    {
        logger.LogInformation(
            "Ping with body endpoint called. Message: {Message}",
            request.Message);

        return Success(
            data: new
            {
                message = request.Message
            },
            message: "Ping thành công");
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Success(
            data: new
            {
                status = "healthy",
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                serverTime = DateTimeOffset.UtcNow
            },
            message: "Lấy data thành công!");
    }
}