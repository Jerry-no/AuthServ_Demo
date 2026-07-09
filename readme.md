# Authentication & Authorization Roadmap

## Mục tiêu

Xây dựng hệ thống Authentication/Authorization single-tenant, production-ready, dựa trên PostgreSQL schema `sys_*`.

Hệ thống cần hỗ trợ đầy đủ:

* User management
* Password authentication
* JWT access token
* Refresh token rotation
* Session management
* Logout từng thiết bị / logout toàn bộ
* RBAC
* ABAC
* MFA
* Recovery codes
* Password reset
* Email / phone verification
* Trusted devices
* Login attempt tracking
* Audit logs
* Authorization decision logs

---

# 1. Nguyên tắc triển khai

## Ưu tiên

Thứ tự code phải đi từ nền tảng đến tính năng cao cấp:

1. Database + Entity mapping
2. User lifecycle
3. Password authentication
4. JWT + refresh token
5. Session management
6. RBAC
7. Audit/security logs
8. Password reset + verification
9. MFA
10. ABAC
11. Trusted devices
12. Hardening + monitoring

Không nên code MFA, ABAC hoặc trusted device trước khi login/session/token ổn định.

---

# 2. Kiến trúc đề xuất

## Solution structure

```text
src/
  Auth.Api/
  Auth.Application/
  Auth.Domain/
  Auth.Infrastructure/
  Auth.Contracts/

tests/
  Auth.UnitTests/
  Auth.IntegrationTests/
```

## Dependency direction

```text
Api -> Application -> Domain
Application -> Infrastructure abstractions only
Infrastructure -> Application + Domain
```

Domain không phụ thuộc EF Core, JWT, Redis, HTTP context.

---

# 3. Phase 1 - Database & Entity Mapping

## Mục tiêu

Map đầy đủ schema sang Entity Framework Core.

## Bảng cần map trước

```text
sys_users
sys_roles
sys_permissions
sys_user_roles
sys_role_permissions
sys_user_sessions
sys_refresh_tokens
sys_invalid_tokens
sys_audit_logs
sys_user_login_attempts
```

## Việc cần code

* Entity classes
* DbContext
* IEntityTypeConfiguration cho từng bảng
* Enum mapping:

    * UserStatus
    * RoleStatus
    * PermissionStatus
    * RefreshTokenStatus
    * AuditActorType
* Repository hoặc Query service
* Unit of Work nếu dùng Clean Architecture

## Acceptance Criteria

* App chạy migration hoặc connect được schema có sẵn.
* Insert/read/update user được.
* Unique username/email hoạt động đúng.
* `updated_at` tự cập nhật từ database trigger.

---

# 4. Phase 2 - User Management

## Mục tiêu

Quản lý vòng đời user.

## API nên có

```text
POST   /api/users
GET    /api/users/{id}
GET    /api/users
PATCH  /api/users/{id}
DELETE /api/users/{id}
PATCH  /api/users/{id}/enable
PATCH  /api/users/{id}/disable
PATCH  /api/users/{id}/lock
PATCH  /api/users/{id}/unlock
```

## Bảng sử dụng

```text
sys_users
sys_audit_logs
```

## Logic quan trọng

* Không hard delete user, chỉ set `is_deleted = true`.
* Username/email dùng case-insensitive.
* Khi disable/lock user, tăng `security_stamp` hoặc `token_version`.
* Audit mọi thay đổi quan trọng.

## Acceptance Criteria

* Không tạo trùng username/email.
* Soft delete xong có thể tạo lại email cũ.
* Disabled user không login được.

---

# 5. Phase 3 - Password Authentication

## Mục tiêu

Login bằng username/email + password.

## API nên có

```text
POST /api/auth/login
POST /api/auth/logout
POST /api/auth/logout-all
```

## Bảng sử dụng

```text
sys_users
sys_user_login_attempts
sys_user_sessions
sys_refresh_tokens
sys_audit_logs
```

## Logic quan trọng

* Hash password bằng Argon2id hoặc BCrypt.
* Không tự implement hashing thủ công.
* Ghi login attempt cho cả success/failure.
* Tăng `failed_login_count` khi login fail.
* Lock user tạm thời nếu vượt ngưỡng.
* Reset `failed_login_count` khi login thành công.
* Kiểm tra:

    * `enabled`
    * `status`
    * `is_deleted`
    * `locked_until`
    * `must_change_password`

## Acceptance Criteria

* Login sai bị ghi log.
* Login đúng tạo session + refresh token.
* User bị lock không login được.
* Có audit cho login/logout.

---

# 6. Phase 4 - JWT Access Token

## Mục tiêu

Phát hành access token ngắn hạn.

## Claims nên có

```text
sub
jti
sid
username
email
security_stamp
token_version
roles
permissions
auth_time
mfa_verified
```

## Bảng sử dụng

```text
sys_user_sessions
sys_invalid_tokens
sys_users
sys_user_roles
sys_role_permissions
sys_permissions
```

## Logic quan trọng

* Access token ngắn hạn: 5-15 phút.
* Validate `jti`.
* Validate `security_stamp`.
* Validate `token_version`.
* Validate session còn active.
* Check `sys_invalid_tokens` nếu logout/revoke.

## Acceptance Criteria

* Access token hết hạn đúng.
* Logout thì access token hiện tại bị invalidate.
* Đổi password thì token cũ không còn hợp lệ.

---

# 7. Phase 5 - Refresh Token Rotation

## Mục tiêu

Refresh token an toàn, có reuse detection.

## API nên có

```text
POST /api/auth/refresh
```

## Bảng sử dụng

```text
sys_refresh_tokens
sys_user_sessions
sys_invalid_tokens
sys_audit_logs
```

## Logic quan trọng

* Refresh token chỉ lưu hash.
* Mỗi lần refresh:

    * token cũ chuyển `USED`
    * set `used_at`
    * set `rotated_at`
    * tạo token mới
    * link `replaced_by_token_id`
* Nếu refresh token đã `USED` mà bị dùng lại:

    * set `reuse_detected_at`
    * status `REUSED`
    * revoke cả token family
    * revoke session
    * ghi audit security event

## Acceptance Criteria

* Refresh token dùng lại bị detect.
* Reuse token làm revoke toàn bộ session family.
* Không bao giờ lưu raw refresh token.

---

# 8. Phase 6 - RBAC

## Mục tiêu

Phân quyền theo role/permission.

## API nên có

```text
POST   /api/roles
GET    /api/roles
PATCH  /api/roles/{id}
DELETE /api/roles/{id}

POST   /api/permissions
GET    /api/permissions

POST   /api/users/{id}/roles/{roleId}
DELETE /api/users/{id}/roles/{roleId}

POST   /api/roles/{id}/permissions/{permissionId}
DELETE /api/roles/{id}/permissions/{permissionId}
```

## Bảng sử dụng

```text
sys_roles
sys_permissions
sys_user_roles
sys_role_permissions
sys_audit_logs
```

## Logic quan trọng

* Không xóa cứng role/permission nếu đang dùng.
* Role system không cho sửa/xóa tùy tiện.
* Permission code nên theo format:

```text
users.read
users.create
users.update
users.delete
roles.manage
auth.sessions.revoke
audit.read
```

## Acceptance Criteria

* User có role thì nhận được permission.
* Permission inactive không có hiệu lực.
* Role inactive không có hiệu lực.

---

# 9. Phase 7 - Authorization Middleware

## Mục tiêu

Kiểm tra permission ở API layer.

## Cần code

* Permission requirement
* Permission authorization handler
* Attribute:

```csharp
[RequirePermission("users.read")]
```

## Bảng sử dụng

```text
sys_users
sys_roles
sys_permissions
sys_user_roles
sys_role_permissions
sys_authorization_decision_logs
```

## Logic quan trọng

* Cache permission bằng Redis hoặc memory cache.
* Invalidate cache khi đổi role/permission.
* Ghi authorization decision log với các request quan trọng.

## Acceptance Criteria

* API bị chặn nếu thiếu permission.
* Decision log ghi allowed/denied.
* Permission cache refresh đúng khi role thay đổi.

---

# 10. Phase 8 - Password Reset & Verification

## Mục tiêu

Hỗ trợ reset password, confirm email, verify phone.

## API nên có

```text
POST /api/auth/forgot-password
POST /api/auth/reset-password
POST /api/auth/send-email-confirmation
POST /api/auth/confirm-email
POST /api/auth/send-phone-verification
POST /api/auth/verify-phone
```

## Bảng sử dụng

```text
sys_user_security_tokens
sys_users
sys_password_history
sys_audit_logs
```

## Logic quan trọng

* Token chỉ lưu hash.
* Token có purpose rõ ràng.
* Token dùng một lần, set `consumed_at`.
* Reset password thành công:

    * update password hash
    * update `password_changed_at`
    * update `password_set_at`
    * tăng `security_stamp`
    * tăng `token_version`
    * revoke session cũ nếu policy yêu cầu
    * lưu password history

## Acceptance Criteria

* Token hết hạn không dùng được.
* Token đã consumed không dùng lại được.
* Reset password làm token/session cũ mất hiệu lực.

---

# 11. Phase 9 - MFA

## Mục tiêu

Hỗ trợ MFA bằng TOTP trước, sau đó recovery code/WebAuthn.

## API nên có

```text
POST /api/mfa/totp/setup
POST /api/mfa/totp/verify
POST /api/mfa/totp/disable

POST /api/mfa/recovery-codes/generate
POST /api/mfa/recovery-codes/verify
```

## Bảng sử dụng

```text
sys_user_mfa_methods
sys_user_recovery_codes
sys_user_sessions
sys_audit_logs
```

## Logic quan trọng

* TOTP secret phải encrypted.
* Recovery code chỉ lưu hash.
* Khi login user có MFA:

    * tạo session nhưng `mfa_verified = false`
    * access token chưa đủ quyền sensitive
    * sau verify MFA mới set `mfa_verified = true`

## Acceptance Criteria

* User bật MFA phải verify sau login.
* Recovery code dùng một lần.
* Disable MFA phải yêu cầu password hoặc MFA hiện tại.

---

# 12. Phase 10 - Trusted Devices

## Mục tiêu

Cho phép nhớ thiết bị tin cậy.

## API nên có

```text
GET    /api/trusted-devices
POST   /api/trusted-devices
DELETE /api/trusted-devices/{id}
```

## Bảng sử dụng

```text
sys_trusted_devices
sys_user_sessions
sys_audit_logs
```

## Logic quan trọng

* Không trust device chỉ bằng user-agent.
* Dùng device fingerprint hash + secure cookie.
* Trusted device phải có expiry.
* Revoke trusted device khi đổi password hoặc detect suspicious login.

## Acceptance Criteria

* Thiết bị hết hạn không còn trusted.
* Revoke device có hiệu lực ngay.
* Audit đầy đủ khi trust/revoke device.

---

# 13. Phase 11 - ABAC

## Mục tiêu

Bổ sung chính sách động ngoài RBAC.

## API nên có

```text
POST   /api/abac/policies
GET    /api/abac/policies
PATCH  /api/abac/policies/{id}
DELETE /api/abac/policies/{id}
```

## Bảng sử dụng

```text
sys_abac_policies
sys_authorization_decision_logs
```

## Logic quan trọng

* RBAC xử lý quyền nền.
* ABAC xử lý điều kiện động:

    * owner only
    * business hours
    * department
    * resource status
    * risk score
    * MFA required
* DENY phải ưu tiên hơn ALLOW.
* Priority nhỏ hơn chạy trước hoặc quy định rõ.

## Acceptance Criteria

* ABAC có thể deny dù RBAC allow.
* Decision log ghi policy match.
* Policy inactive không được evaluate.

---

# 14. Phase 12 - Audit, Monitoring, Security Hardening

## Mục tiêu

Đưa hệ thống về trạng thái production-ready.

## Cần làm

* Centralized audit service
* Security event service
* Correlation ID middleware
* Trace ID middleware
* Rate limiting
* IP-based throttling
* Account lockout policy
* Password policy
* Token cleanup background job
* Session cleanup background job
* Audit log partitioning
* Metrics

## Bảng sử dụng

```text
sys_audit_logs
sys_authorization_decision_logs
sys_user_login_attempts
sys_invalid_tokens
sys_refresh_tokens
sys_user_sessions
```

## Background jobs

```text
CleanupExpiredRefreshTokens
CleanupExpiredInvalidTokens
CleanupExpiredSecurityTokens
CleanupExpiredSessions
MarkExpiredRefreshTokens
RevokeStaleSessions
PartitionAuditLogs
```

## Metrics nên có

```text
auth_login_success_total
auth_login_failure_total
auth_token_refresh_total
auth_token_reuse_detected_total
auth_mfa_success_total
auth_mfa_failure_total
auth_authorization_denied_total
auth_active_sessions
```

---

# 15. Thứ tự triển khai khuyến nghị

## Sprint 1 - Core Foundation

* Entity mapping
* DbContext
* User repository
* Password hasher
* Audit service base
* User create/update/delete

## Sprint 2 - Login & Session

* Login API
* Login attempt tracking
* Session creation
* JWT access token
* Refresh token creation
* Logout API

## Sprint 3 - Refresh Token Rotation

* Refresh API
* Token rotation
* Reuse detection
* Revoke session family
* Logout all devices

## Sprint 4 - RBAC

* Role CRUD
* Permission CRUD
* Assign role to user
* Assign permission to role
* Permission claims
* Permission authorization handler

## Sprint 5 - Security Tokens

* Forgot password
* Reset password
* Email confirmation
* Phone verification
* Password history

## Sprint 6 - MFA

* TOTP setup
* TOTP verify
* MFA login challenge
* Recovery codes
* MFA audit

## Sprint 7 - Trusted Devices

* Trust device
* List devices
* Revoke device
* Device expiry
* Suspicious login handling

## Sprint 8 - ABAC

* ABAC policy CRUD
* Policy evaluator
* DENY/ALLOW priority
* Decision logging

## Sprint 9 - Production Hardening

* Rate limiting
* Cleanup jobs
* Monitoring metrics
* Audit partitioning
* Security test
* Load test

---

# 16. Không nên làm sớm

Không nên code các phần sau ở giai đoạn đầu:

* ABAC engine phức tạp
* WebAuthn
* Risk engine nâng cao
* Distributed session invalidation
* Admin UI
* Advanced device fingerprinting

Các phần này phụ thuộc nền login/session/token/RBAC ổn định.

---

# 17. Definition of Done

Một phase chỉ được xem là hoàn thành khi có đủ:

* API
* Application command/query
* Domain rule
* Infrastructure implementation
* Unit test
* Integration test
* Audit log
* Error handling
* Validation
* Security review cơ bản

---

# 18. Security Rules

## Password

* Không lưu plain password.
* Không log password.
* Hash bằng Argon2id hoặc BCrypt.
* Có password history nếu cần policy chống reuse.

## Token

* Refresh token chỉ lưu hash.
* Access token ngắn hạn.
* Refresh token phải rotate.
* Detect reuse phải revoke cả family.

## Audit

* Login/logout/reset password/MFA/role changes phải audit.
* Audit log không được update/delete từ application.
* Mọi request quan trọng phải có correlation ID.

## Authorization

* Không tin role/permission từ client.
* Permission phải resolve từ server.
* Sensitive API nên yêu cầu MFA verified.

---

# 19. Recommendation

Triển khai tối thiểu production nên hoàn thành đến hết Sprint 6:

```text
User
Password login
JWT
Refresh token rotation
Session management
RBAC
Password reset
Email verification
MFA TOTP
Audit log
Login attempt tracking
```

ABAC và trusted devices có thể làm sau nếu business chưa cần ngay.
