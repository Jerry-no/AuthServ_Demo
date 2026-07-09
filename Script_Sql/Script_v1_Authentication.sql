CREATE EXTENSION IF NOT EXISTS pgcrypto;
CREATE EXTENSION IF NOT EXISTS citext;

-- =========================
-- COMMON UPDATED_AT TRIGGER
-- =========================
CREATE OR REPLACE FUNCTION set_updated_at()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- =========================
-- USERS
-- =========================
CREATE TABLE sys_users (
                           id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

                           username CITEXT NOT NULL,
                           email CITEXT,
                           phone_number VARCHAR(30),

                           password_hash VARCHAR(500) NOT NULL,
                           password_algorithm VARCHAR(50) NOT NULL DEFAULT 'ARGON2ID',
                           password_hash_version INT NOT NULL DEFAULT 1,
                           password_set_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
                           password_changed_at TIMESTAMPTZ,
                           password_expires_at TIMESTAMPTZ,
                           must_change_password BOOLEAN NOT NULL DEFAULT FALSE,

                           full_name VARCHAR(255),

                           email_verified_at TIMESTAMPTZ,
                           phone_verified_at TIMESTAMPTZ,

                           enabled BOOLEAN NOT NULL DEFAULT TRUE,
                           status VARCHAR(50) NOT NULL DEFAULT 'ACTIVE',

                           last_login_at TIMESTAMPTZ,
                           last_failed_login_at TIMESTAMPTZ,
                           failed_login_count INT NOT NULL DEFAULT 0,
                           locked_until TIMESTAMPTZ,

                           mfa_enabled BOOLEAN NOT NULL DEFAULT FALSE,
                           mfa_verified_at TIMESTAMPTZ,

                           security_stamp UUID NOT NULL DEFAULT gen_random_uuid(),
                           token_version INT NOT NULL DEFAULT 0,

                           created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
                           updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    
                           created_by UUID REFERENCES sys_users(id) ON DELETE SET NULL,
                           updated_by UUID REFERENCES sys_users(id) ON DELETE SET NULL,

                           noted TEXT,
                           metadata JSONB NOT NULL DEFAULT '{}'::jsonb,
                           is_deleted BOOLEAN NOT NULL DEFAULT FALSE,

                           CONSTRAINT chk_sys_users_status
                               CHECK (status IN ('ACTIVE', 'INACTIVE', 'LOCKED', 'SUSPENDED', 'PENDING')),

                           CONSTRAINT chk_sys_users_failed_login_count
                               CHECK (failed_login_count >= 0)
);

CREATE UNIQUE INDEX ux_sys_users_username_active
    ON sys_users (username)
    WHERE is_deleted = FALSE;

CREATE UNIQUE INDEX ux_sys_users_email_active
    ON sys_users (email)
    WHERE email IS NOT NULL AND is_deleted = FALSE;

CREATE UNIQUE INDEX ux_sys_users_phone_active
    ON sys_users (phone_number)
    WHERE phone_number IS NOT NULL AND is_deleted = FALSE;

CREATE INDEX ix_sys_users_status
    ON sys_users (status, enabled, is_deleted);

CREATE TRIGGER trg_sys_users_updated_at
    BEFORE UPDATE ON sys_users
    FOR EACH ROW EXECUTE FUNCTION set_updated_at();

-- =========================
-- ROLES
-- =========================
CREATE TABLE sys_roles (
                           id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

                           code CITEXT NOT NULL,
                           name VARCHAR(100) NOT NULL,
                           description TEXT,

                           is_system BOOLEAN NOT NULL DEFAULT FALSE,
                           status VARCHAR(20) NOT NULL DEFAULT 'ACTIVE',

                           created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
                           updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    
                           created_by UUID REFERENCES sys_users(id) ON DELETE SET NULL,
                           updated_by UUID REFERENCES sys_users(id) ON DELETE SET NULL,

                           noted TEXT,
                           metadata JSONB NOT NULL DEFAULT '{}'::jsonb,
                           is_deleted BOOLEAN NOT NULL DEFAULT FALSE,

                           CONSTRAINT chk_sys_roles_status
                               CHECK (status IN ('ACTIVE', 'INACTIVE'))
);

CREATE UNIQUE INDEX ux_sys_roles_code_active
    ON sys_roles (code)
    WHERE is_deleted = FALSE;

CREATE INDEX ix_sys_roles_status
    ON sys_roles (status, is_deleted);

CREATE TRIGGER trg_sys_roles_updated_at
    BEFORE UPDATE ON sys_roles
    FOR EACH ROW EXECUTE FUNCTION set_updated_at();

-- =========================
-- PERMISSIONS
-- =========================
CREATE TABLE sys_permissions (
                                 id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

                                 code CITEXT NOT NULL,
                                 name VARCHAR(150) NOT NULL,

                                 resource_type VARCHAR(100) NOT NULL,
                                 action VARCHAR(100) NOT NULL,

                                 description TEXT,
                                 status VARCHAR(20) NOT NULL DEFAULT 'ACTIVE',

                                 created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
                                 updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),

                                 created_by UUID REFERENCES sys_users(id) ON DELETE SET NULL,
                                 updated_by UUID REFERENCES sys_users(id) ON DELETE SET NULL,
    
                                 noted TEXT,
                                 metadata JSONB NOT NULL DEFAULT '{}'::jsonb,
                                 is_deleted BOOLEAN NOT NULL DEFAULT FALSE,

                                 CONSTRAINT chk_sys_permissions_status
                                     CHECK (status IN ('ACTIVE', 'INACTIVE'))
);

CREATE UNIQUE INDEX ux_sys_permissions_code_active
    ON sys_permissions (code)
    WHERE is_deleted = FALSE;

CREATE UNIQUE INDEX ux_sys_permissions_resource_action_active
    ON sys_permissions (resource_type, action)
    WHERE is_deleted = FALSE;

CREATE INDEX ix_sys_permissions_lookup
    ON sys_permissions (resource_type, action, status);

CREATE TRIGGER trg_sys_permissions_updated_at
    BEFORE UPDATE ON sys_permissions
    FOR EACH ROW EXECUTE FUNCTION set_updated_at();

-- =========================
-- USER ROLES
-- =========================
CREATE TABLE sys_user_roles (
                                user_id UUID NOT NULL REFERENCES sys_users(id) ON DELETE CASCADE,
                                role_id UUID NOT NULL REFERENCES sys_roles(id) ON DELETE CASCADE,

                                status VARCHAR(20) NOT NULL DEFAULT 'ACTIVE',

                                assigned_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
                                assigned_by UUID REFERENCES sys_users(id) ON DELETE SET NULL,

                                updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
                                updated_by UUID REFERENCES sys_users(id) ON DELETE SET NULL,

                                noted TEXT,

                                PRIMARY KEY (user_id, role_id),

                                CONSTRAINT chk_sys_user_roles_status
                                    CHECK (status IN ('ACTIVE', 'INACTIVE', 'REVOKED'))
);

CREATE INDEX ix_sys_user_roles_role
    ON sys_user_roles (role_id, status);

CREATE TRIGGER trg_sys_user_roles_updated_at
    BEFORE UPDATE ON sys_user_roles
    FOR EACH ROW EXECUTE FUNCTION set_updated_at();

-- =========================
-- ROLE PERMISSIONS
-- =========================
CREATE TABLE sys_role_permissions (
                                      role_id UUID NOT NULL REFERENCES sys_roles(id) ON DELETE CASCADE,
                                      permission_id UUID NOT NULL REFERENCES sys_permissions(id) ON DELETE CASCADE,

                                      status VARCHAR(20) NOT NULL DEFAULT 'ACTIVE',

                                      assigned_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
                                      assigned_by UUID REFERENCES sys_users(id) ON DELETE SET NULL,

                                      noted TEXT,

                                      PRIMARY KEY (role_id, permission_id),

                                      CONSTRAINT chk_sys_role_permissions_status
                                          CHECK (status IN ('ACTIVE', 'INACTIVE', 'REVOKED'))
);

CREATE INDEX ix_sys_role_permissions_permission
    ON sys_role_permissions (permission_id, status);

-- =========================
-- USER SESSIONS
-- =========================
CREATE TABLE sys_user_sessions (
                                   id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

                                   user_id UUID NOT NULL REFERENCES sys_users(id) ON DELETE CASCADE,

                                   access_token_jti UUID NOT NULL UNIQUE,
                                   session_family_id UUID NOT NULL DEFAULT gen_random_uuid(),
                                   refresh_token_family_id UUID NOT NULL DEFAULT gen_random_uuid(),

                                   ip_address INET,
                                   user_agent TEXT,

                                   device_id VARCHAR(255),
                                   device_name VARCHAR(255),
                                   device_fingerprint_hash VARCHAR(255),

                                   mfa_verified BOOLEAN NOT NULL DEFAULT FALSE,
                                   risk_score NUMERIC(5,2),

                                   auth_time TIMESTAMPTZ NOT NULL DEFAULT NOW(),

                                   created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
                                   expires_at TIMESTAMPTZ NOT NULL,
                                   absolute_expires_at TIMESTAMPTZ,
                                   idle_expires_at TIMESTAMPTZ,

                                   last_accessed_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
                                   last_seen_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),

                                   is_active BOOLEAN NOT NULL DEFAULT TRUE,

                                   revoked_at TIMESTAMPTZ,
                                   revoked_by UUID REFERENCES sys_users(id) ON DELETE SET NULL,
                                   revoked_reason TEXT,

                                   logout_at TIMESTAMPTZ,

                                   noted TEXT,
                                   metadata JSONB NOT NULL DEFAULT '{}'::jsonb,

                                   CONSTRAINT chk_user_sessions_expiry
                                       CHECK (expires_at > created_at),

                                   CONSTRAINT chk_user_sessions_risk_score
                                       CHECK (risk_score IS NULL OR (risk_score >= 0 AND risk_score <= 100))
);

CREATE INDEX ix_user_sessions_user
    ON sys_user_sessions (user_id, is_active);

CREATE INDEX ix_user_sessions_family
    ON sys_user_sessions (refresh_token_family_id);

CREATE INDEX ix_user_sessions_expires_at
    ON sys_user_sessions (expires_at);

-- =========================
-- REFRESH TOKENS
-- =========================
CREATE TABLE sys_refresh_tokens (
                                    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

                                    session_id UUID NOT NULL REFERENCES sys_user_sessions(id) ON DELETE CASCADE,
                                    user_id UUID NOT NULL REFERENCES sys_users(id) ON DELETE CASCADE,

                                    family_id UUID NOT NULL,

                                    token_hash VARCHAR(500) NOT NULL UNIQUE,
                                    jti UUID NOT NULL UNIQUE,

                                    parent_token_id UUID REFERENCES sys_refresh_tokens(id) ON DELETE SET NULL,
                                    replaced_by_token_id UUID REFERENCES sys_refresh_tokens(id) ON DELETE SET NULL,

                                    expires_at TIMESTAMPTZ NOT NULL,

                                    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
                                    created_by_ip INET,

                                    used_at TIMESTAMPTZ,
                                    rotated_at TIMESTAMPTZ,

                                    revoked_at TIMESTAMPTZ,
                                    revoked_by_ip INET,
                                    revoked_reason TEXT,

                                    reuse_detected_at TIMESTAMPTZ,

                                    status VARCHAR(20) NOT NULL DEFAULT 'ACTIVE',

                                    metadata JSONB NOT NULL DEFAULT '{}'::jsonb,

                                    CONSTRAINT chk_refresh_tokens_expiry
                                        CHECK (expires_at > created_at),

                                    CONSTRAINT chk_refresh_tokens_status
                                        CHECK (status IN ('ACTIVE', 'USED', 'REVOKED', 'EXPIRED', 'REUSED'))
);

CREATE INDEX ix_refresh_tokens_session
    ON sys_refresh_tokens (session_id);

CREATE INDEX ix_refresh_tokens_user
    ON sys_refresh_tokens (user_id);

CREATE INDEX ix_refresh_tokens_family
    ON sys_refresh_tokens (family_id);

CREATE INDEX ix_refresh_tokens_expires_at
    ON sys_refresh_tokens (expires_at);

CREATE INDEX ix_refresh_tokens_status
    ON sys_refresh_tokens (status);

-- =========================
-- INVALID ACCESS TOKENS
-- =========================
CREATE TABLE sys_invalid_tokens (
                                    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

                                    jti UUID NOT NULL UNIQUE,

                                    user_id UUID REFERENCES sys_users(id) ON DELETE SET NULL,
                                    session_id UUID REFERENCES sys_user_sessions(id) ON DELETE CASCADE,

                                    reason TEXT,

                                    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
                                    expires_at TIMESTAMPTZ NOT NULL,

                                    CONSTRAINT chk_invalid_tokens_expiry
                                        CHECK (expires_at > created_at)
);

CREATE INDEX ix_invalid_tokens_expires_at
    ON sys_invalid_tokens (expires_at);

CREATE INDEX ix_invalid_tokens_user
    ON sys_invalid_tokens (user_id);

-- =========================
-- MFA METHODS
-- =========================
CREATE TABLE sys_user_mfa_methods (
                                      id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

                                      user_id UUID NOT NULL REFERENCES sys_users(id) ON DELETE CASCADE,

                                      method_type VARCHAR(30) NOT NULL,

                                      secret_encrypted TEXT,
                                      credential_id TEXT,
                                      public_key TEXT,

                                      display_name VARCHAR(150),

                                      enabled BOOLEAN NOT NULL DEFAULT TRUE,
                                      verified_at TIMESTAMPTZ,
                                      last_used_at TIMESTAMPTZ,

                                      created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
                                      updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),

                                      metadata JSONB NOT NULL DEFAULT '{}'::jsonb,

                                      CONSTRAINT chk_user_mfa_methods_type
                                          CHECK (method_type IN ('TOTP', 'EMAIL', 'SMS', 'WEBAUTHN')),

                                      CONSTRAINT chk_user_mfa_methods_verified
                                          CHECK (
                                              enabled = FALSE
                                                  OR verified_at IS NOT NULL
                                                  OR method_type IN ('EMAIL', 'SMS')
                                              )
);

CREATE INDEX ix_user_mfa_methods_user
    ON sys_user_mfa_methods (user_id, enabled);

CREATE UNIQUE INDEX ux_user_mfa_webauthn_credential
    ON sys_user_mfa_methods (credential_id)
    WHERE credential_id IS NOT NULL;

CREATE TRIGGER trg_user_mfa_methods_updated_at
    BEFORE UPDATE ON sys_user_mfa_methods
    FOR EACH ROW EXECUTE FUNCTION set_updated_at();

-- =========================
-- MFA RECOVERY CODES
-- =========================
CREATE TABLE sys_user_recovery_codes (
                                         id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

                                         user_id UUID NOT NULL REFERENCES sys_users(id) ON DELETE CASCADE,

                                         code_hash VARCHAR(500) NOT NULL UNIQUE,

                                         used_at TIMESTAMPTZ,
                                         used_by_ip INET,

                                         created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
                                         expires_at TIMESTAMPTZ,

                                         CONSTRAINT chk_recovery_codes_expiry
                                             CHECK (expires_at IS NULL OR expires_at > created_at)
);

CREATE INDEX ix_user_recovery_codes_user
    ON sys_user_recovery_codes (user_id, used_at);

-- =========================
-- SECURITY TOKENS
-- email confirmation, phone verification, reset password, invite, magic link
-- =========================
CREATE TABLE sys_user_security_tokens (
                                          id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

                                          user_id UUID REFERENCES sys_users(id) ON DELETE CASCADE,

                                          purpose VARCHAR(50) NOT NULL,
                                          token_hash VARCHAR(500) NOT NULL UNIQUE,

                                          target_value VARCHAR(255),

                                          created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
                                          expires_at TIMESTAMPTZ NOT NULL,

                                          consumed_at TIMESTAMPTZ,

                                          created_by_ip INET,
                                          consumed_by_ip INET,

                                          user_agent TEXT,

                                          metadata JSONB NOT NULL DEFAULT '{}'::jsonb,

                                          CONSTRAINT chk_security_tokens_purpose
                                              CHECK (purpose IN (
                                                                 'EMAIL_CONFIRMATION',
                                                                 'PHONE_VERIFICATION',
                                                                 'PASSWORD_RESET',
                                                                 'INVITATION',
                                                                 'MAGIC_LINK',
                                                                 'MFA_CHALLENGE'
                                                  )),

                                          CONSTRAINT chk_security_tokens_expiry
                                              CHECK (expires_at > created_at)
);

CREATE INDEX ix_security_tokens_user
    ON sys_user_security_tokens (user_id, purpose, consumed_at);

CREATE INDEX ix_security_tokens_expires_at
    ON sys_user_security_tokens (expires_at);

-- =========================
-- LOGIN ATTEMPTS
-- =========================
CREATE TABLE sys_user_login_attempts (
                                         id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

                                         user_id UUID REFERENCES sys_users(id) ON DELETE SET NULL,

                                         username_or_email CITEXT,

                                         success BOOLEAN NOT NULL,
                                         failure_reason VARCHAR(100),

                                         ip_address INET,
                                         user_agent TEXT,

                                         device_id VARCHAR(255),
                                         device_fingerprint_hash VARCHAR(255),

                                         risk_score NUMERIC(5,2),

                                         trace_id VARCHAR(100),
                                         correlation_id VARCHAR(100),

                                         created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),

                                         metadata JSONB NOT NULL DEFAULT '{}'::jsonb,

                                         CONSTRAINT chk_login_attempts_risk_score
                                             CHECK (risk_score IS NULL OR (risk_score >= 0 AND risk_score <= 100))
);

CREATE INDEX ix_login_attempts_user_created
    ON sys_user_login_attempts (user_id, created_at DESC);

CREATE INDEX ix_login_attempts_identifier_created
    ON sys_user_login_attempts (username_or_email, created_at DESC);

CREATE INDEX ix_login_attempts_ip_created
    ON sys_user_login_attempts (ip_address, created_at DESC);

CREATE INDEX ix_login_attempts_success_created
    ON sys_user_login_attempts (success, created_at DESC);

-- =========================
-- PASSWORD HISTORY
-- =========================
CREATE TABLE sys_password_history (
                                      id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

                                      user_id UUID NOT NULL REFERENCES sys_users(id) ON DELETE CASCADE,

                                      password_hash VARCHAR(500) NOT NULL,
                                      password_algorithm VARCHAR(50) NOT NULL,
                                      password_hash_version INT NOT NULL,

                                      created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
                                      created_by_ip INET
);

CREATE INDEX ix_password_history_user_created
    ON sys_password_history (user_id, created_at DESC);

-- =========================
-- TRUSTED DEVICES
-- =========================
CREATE TABLE sys_trusted_devices (
                                     id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

                                     user_id UUID NOT NULL REFERENCES sys_users(id) ON DELETE CASCADE,

                                     device_id VARCHAR(255) NOT NULL,
                                     device_name VARCHAR(255),
                                     device_fingerprint_hash VARCHAR(255) NOT NULL,

                                     trusted_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
                                     expires_at TIMESTAMPTZ NOT NULL,

                                     last_used_at TIMESTAMPTZ,

                                     revoked_at TIMESTAMPTZ,
                                     revoked_reason TEXT,

                                     ip_address INET,
                                     user_agent TEXT,

                                     metadata JSONB NOT NULL DEFAULT '{}'::jsonb,

                                     CONSTRAINT chk_trusted_devices_expiry
                                         CHECK (expires_at > trusted_at)
);

CREATE UNIQUE INDEX ux_trusted_devices_user_device_active
    ON sys_trusted_devices (user_id, device_id)
    WHERE revoked_at IS NULL;

CREATE INDEX ix_trusted_devices_user
    ON sys_trusted_devices (user_id, revoked_at, expires_at);

-- =========================
-- ABAC POLICIES
-- =========================
CREATE TABLE sys_abac_policies (
                                   id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

                                   name VARCHAR(150) NOT NULL,
                                   code CITEXT NOT NULL,

                                   resource_type VARCHAR(100) NOT NULL,
                                   action VARCHAR(100) NOT NULL,

                                   effect VARCHAR(20) NOT NULL DEFAULT 'ALLOW',
                                   priority INT NOT NULL DEFAULT 100,
                                   version INT NOT NULL DEFAULT 1,

                                   condition_expression JSONB NOT NULL DEFAULT '{}'::jsonb,

                                   status VARCHAR(20) NOT NULL DEFAULT 'ACTIVE',

                                   created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
                                   updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),

                                   created_by UUID REFERENCES sys_users(id) ON DELETE SET NULL,
                                   updated_by UUID REFERENCES sys_users(id) ON DELETE SET NULL,

                                   noted TEXT,
                                   metadata JSONB NOT NULL DEFAULT '{}'::jsonb,
                                   is_deleted BOOLEAN NOT NULL DEFAULT FALSE,

                                   CONSTRAINT chk_abac_policy_effect
                                       CHECK (effect IN ('ALLOW', 'DENY')),

                                   CONSTRAINT chk_abac_policy_status
                                       CHECK (status IN ('ACTIVE', 'INACTIVE'))
);

CREATE UNIQUE INDEX ux_abac_policies_code_version_active
    ON sys_abac_policies (code, version)
    WHERE is_deleted = FALSE;

CREATE INDEX ix_abac_policies_lookup
    ON sys_abac_policies (resource_type, action, status, priority)
    WHERE is_deleted = FALSE;

CREATE TRIGGER trg_abac_policies_updated_at
    BEFORE UPDATE ON sys_abac_policies
    FOR EACH ROW EXECUTE FUNCTION set_updated_at();

-- =========================
-- AUTHORIZATION DECISION LOGS
-- =========================
CREATE TABLE sys_authorization_decision_logs (
                                                 id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

                                                 user_id UUID REFERENCES sys_users(id) ON DELETE SET NULL,
                                                 session_id UUID REFERENCES sys_user_sessions(id) ON DELETE SET NULL,

                                                 resource_type VARCHAR(100) NOT NULL,
                                                 resource_id UUID,
                                                 action VARCHAR(100) NOT NULL,

                                                 allowed BOOLEAN NOT NULL,

                                                 matched_policy_id UUID REFERENCES sys_abac_policies(id) ON DELETE SET NULL,
                                                 deny_reason TEXT,

                                                 subject_attributes JSONB NOT NULL DEFAULT '{}'::jsonb,
                                                 resource_attributes JSONB NOT NULL DEFAULT '{}'::jsonb,
                                                 environment_attributes JSONB NOT NULL DEFAULT '{}'::jsonb,

                                                 ip_address INET,
                                                 user_agent TEXT,

                                                 trace_id VARCHAR(100),
                                                 correlation_id VARCHAR(100),

                                                 evaluation_time_ms INT,

                                                 created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX ix_auth_decision_logs_user_created
    ON sys_authorization_decision_logs (user_id, created_at DESC);

CREATE INDEX ix_auth_decision_logs_resource
    ON sys_authorization_decision_logs (resource_type, resource_id, action);

CREATE INDEX ix_auth_decision_logs_allowed
    ON sys_authorization_decision_logs (allowed, created_at DESC);

CREATE INDEX ix_auth_decision_logs_trace
    ON sys_authorization_decision_logs (trace_id);

-- =========================
-- GENERAL AUDIT LOGS
-- =========================
CREATE TABLE sys_audit_logs (
                                id UUID PRIMARY KEY DEFAULT gen_random_uuid(),

                                user_id UUID REFERENCES sys_users(id) ON DELETE SET NULL,
                                session_id UUID REFERENCES sys_user_sessions(id) ON DELETE SET NULL,

                                actor_type VARCHAR(50) NOT NULL DEFAULT 'USER',

                                action VARCHAR(100) NOT NULL,
                                resource_type VARCHAR(100),
                                resource_id UUID,

                                http_method VARCHAR(20),
                                request_path TEXT,

                                ip_address INET,
                                user_agent TEXT,

                                success BOOLEAN NOT NULL,
                                error_message TEXT,

                                before_data JSONB,
                                after_data JSONB,

                                trace_id VARCHAR(100),
                                correlation_id VARCHAR(100),

                                metadata JSONB NOT NULL DEFAULT '{}'::jsonb,

                                hash VARCHAR(128),
                                previous_hash VARCHAR(128),

                                status VARCHAR(50) NOT NULL DEFAULT 'ACTIVE',

                                created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
                                created_by UUID REFERENCES sys_users(id) ON DELETE SET NULL,

                                noted TEXT,

                                CONSTRAINT chk_audit_logs_actor_type
                                    CHECK (actor_type IN ('USER', 'SYSTEM', 'SERVICE', 'ANONYMOUS'))
);

CREATE INDEX ix_audit_logs_user_created
    ON sys_audit_logs (user_id, created_at DESC);

CREATE INDEX ix_audit_logs_resource
    ON sys_audit_logs (resource_type, resource_id);

CREATE INDEX ix_audit_logs_trace
    ON sys_audit_logs (trace_id);

CREATE INDEX ix_audit_logs_action_created
    ON sys_audit_logs (action, created_at DESC);

-- Optional append-only protection for audit logs
CREATE OR REPLACE FUNCTION prevent_audit_log_mutation()
RETURNS TRIGGER AS $$
BEGIN
    RAISE EXCEPTION 'sys_audit_logs is append-only';
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_prevent_audit_log_update
    BEFORE UPDATE ON sys_audit_logs
    FOR EACH ROW EXECUTE FUNCTION prevent_audit_log_mutation();

CREATE TRIGGER trg_prevent_audit_log_delete
    BEFORE DELETE ON sys_audit_logs
    FOR EACH ROW EXECUTE FUNCTION prevent_audit_log_mutation();