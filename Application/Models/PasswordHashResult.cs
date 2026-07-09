namespace AuthService.Application.Models;

public sealed record PasswordHashResult(
    string Hash,
    string Algorithm,
    int Version);