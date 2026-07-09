using System.Net;
using AuthService.Common.Models;

namespace AuthService.Domain.Entity;


public sealed class UserRecoveryCode
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string CodeHash { get; set; } = null!;

    public DateTimeOffset? UsedAt { get; set; }

    public IPAddress? UsedByIp { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? ExpiresAt { get; set; }

    public User User { get; set; } = null!;
}