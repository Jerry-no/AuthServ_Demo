using AuthService.Common.Abstractions.Messaging;

namespace AuthService.Application.Auth.Commands.Login;

public sealed record LoginCommand(
    string UsernameOrEmail,
    string Password
) : ICommand<AuthResponse>;