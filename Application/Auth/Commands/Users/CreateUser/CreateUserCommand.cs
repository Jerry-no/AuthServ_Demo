using System.Text.Json;
using AuthService.Common.Abstractions.Messaging;
using AuthService.Common.Constants;

namespace AuthService.Application.Auth.Commands.Users.CreateUser;

public sealed record CreateUserCommand(
    string Username,
    string Password,
    string? Email,
    string? PhoneNumber,
    string? FullName,
    bool Enabled,
    UserStatus Status,
    bool MustChangePassword,
    JsonDocument? Metadata
) : ICommand<int>;