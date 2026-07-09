using System.Text.Json;
using AuthService.Common.Constants;
using AuthService.Common.Models;

namespace AuthService.Domain.Entity;

public sealed class User : EntityBase
{
    public string Username { get; set; } = null!;

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string PasswordAlgorithm { get; set; } = null!;

    public int PasswordHashVersion { get; set; }

    public DateTimeOffset PasswordSetAt { get; set; }

    public DateTimeOffset? PasswordChangedAt { get; set; }

    public DateTimeOffset? PasswordExpiresAt { get; set; }

    public bool MustChangePassword { get; set; }

    public string? FullName { get; set; }

    public DateTimeOffset? EmailVerifiedAt { get; set; }

    public DateTimeOffset? PhoneVerifiedAt { get; set; }

    public bool Enabled { get; set; }

    public UserStatus Status { get; set; } 

    public DateTimeOffset? LastLoginAt { get; set; }

    public DateTimeOffset? LastFailedLoginAt { get; set; }

    public int FailedLoginCount { get; set; }

    public DateTimeOffset? LockedUntil { get; set; }

    public bool MfaEnabled { get; set; }

    public DateTimeOffset? MfaVerifiedAt { get; set; }

    public Guid SecurityStamp { get; set; }

    public int TokenVersion { get; set; }
    
    public Guid? CreatedBy { get; set; }
    
    public Guid? UpdatedBy { get; set; }
    public JsonDocument  Metadata { get; set; }  = JsonDocument.Parse("{}");
    public ICollection<UserRole> UserRoles { get; set; } = [];

    public ICollection<UserSession> Sessions { get; set; } = [];

    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    
    public ICollection<UserMfaMethod> MfaMethods { get; set; } = [];
    
    public ICollection<UserRecoveryCode> RecoveryCodes { get; set; } = [];
    
    public ICollection<UserSecurityToken> SecurityTokens { get; set; } = [];
    
    public ICollection<UserLoginAttempt> LoginAttempts { get; set; } = [];
    
    public ICollection<PasswordHistory> PasswordHistories { get; set; } = [];
    
    public ICollection<TrustedDevice> TrustedDevices { get; set; } = [];
    
    public ICollection<AuthorizationDecisionLog> AuthorizationDecisionLogs { get; set; } = [];
}