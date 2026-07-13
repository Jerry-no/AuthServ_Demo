using AuthService.Application.Auth.Commands.Login;
using AuthService.Application.Auth.Responses;
using AuthService.Domain.Entity;

namespace AuthService.Domain.Interfaces;

public interface ITokenService
{
    Task<LoginResponse> GenerateAsync(
        User user,
        CancellationToken cancellationToken = default);
}