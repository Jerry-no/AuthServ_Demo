namespace AuthService.Common.Constants;

public enum UserStatus
{
    Pending,

    Active,
    
    Inactive,

    Locked,

    Suspended
}

public enum RoleStatus
{
    Active,
    Disabled,
    Archived
}

public enum PermissionStatus
{
    Active,
    Disabled,
    Archived
}


public enum UserRoleStatus
{
    Active,
    Revoked,
    Expired,
    Disabled
}
public enum RolePermissionStatus
{
    Active,
    Revoked,
    Disabled
}

public enum RefreshTokenStatus
{
    Active,
    Used,
    Revoked,
    Expired,
    Reused
}

public enum UserMfaMethodType
{
    Totp,
    Email,
    Sms,
    WebAuthn
}

public enum SecurityTokenPurpose
{
    EmailConfirmation,
    PhoneVerification,
    PasswordReset,
    Invitation,
    MagicLink,
    MfaChallenge
}

public enum AbacPolicyEffect
{
    Allow,
    Deny
}

public enum AbacPolicyStatus
{
    Active,
    Inactive
}

public enum AuditActorType
{
    User,
    System,
    Service,
    Anonymous
}

public enum AuditLogStatus
{
    Active,
    Archived
}