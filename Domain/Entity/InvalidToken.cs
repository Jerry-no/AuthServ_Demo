using AuthService.Common.Models;

namespace AuthService.Domain.Entity;


public sealed class InvalidToken
{
    public Guid Id { get; set; }

    public Guid Jti { get; set; }

    public Guid? UserId { get; set; }

    public Guid? SessionId { get; set; }

    public string? Reason { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ExpiresAt { get; set; }

    public User? User { get; set; }

    public UserSession? Session { get; set; }
}