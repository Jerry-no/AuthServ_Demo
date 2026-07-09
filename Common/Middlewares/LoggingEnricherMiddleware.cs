using System.Security.Claims;
using Serilog.Context;

namespace AuthService.Common.Middlewares;

public sealed class LoggingEnricherMiddleware(RequestDelegate next)
{
    private const string TenantClaimName = "x-code";

    public async Task InvokeAsync(HttpContext context)
    {
        var traceId = context.TraceIdentifier;
        var tenantCode = GetTenantCode(context);
        var userId = GetUserId(context);

        using (LogContext.PushProperty("TraceId", traceId))
        using (LogContext.PushProperty("TenantCode", tenantCode))
        using (LogContext.PushProperty("UserId", userId))
        {
            await next(context);
        }
    }

    private static string GetTenantCode(HttpContext context)
    {
        return context.User.FindFirstValue(TenantClaimName)
               ?? context.Request.Headers["X-Code"].FirstOrDefault()
               ?? "anonymous";
    }

    private static string GetUserId(HttpContext context)
    {
        return context.User.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? context.User.FindFirstValue("sub")
               ?? "anonymous";
    }
}