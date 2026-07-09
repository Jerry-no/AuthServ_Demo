using AuthService.Application.Auth.Commands.Login;
using AuthService.Common.Abstractions.Messaging;

namespace AuthService.Application.Auth.Commands.Register;

public sealed record RegisterCommand(
    string Username,
    string Password,
    string? Email,
    string? PhoneNumber,
    string? FullName
) : ICommand<AuthResponse>;