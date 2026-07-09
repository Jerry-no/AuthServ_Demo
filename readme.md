# Authentication & Authorization Roadmap

## Mục tiêu

Hệ thống hỗ trợ đầy đủ:

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


