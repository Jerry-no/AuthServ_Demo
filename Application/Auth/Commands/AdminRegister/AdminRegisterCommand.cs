using AuthService.Application.Auth.Commands.Login;
using AuthService.Common.Abstractions.Messaging;

namespace AuthService.Application.Auth.Commands.AdminRegister;

public sealed record AdminRegisterCommand(
    AdminRegisterRequest AdminRegisterRequest
) : ICommand<LoginResponse>;