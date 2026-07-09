using System.Net;

namespace AuthService.Domain.Entity;

public sealed class PasswordHistory
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string PasswordAlgorithm { get; set; } = null!;

    public int PasswordHashVersion { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public IPAddress? CreatedByIp { get; set; }

    public User User { get; set; } = null!;
}