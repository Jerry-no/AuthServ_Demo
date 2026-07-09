using AuthService.Application.Auth.Queries.Users.GetUsers;
using AuthService.Common.Security;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Security;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Common.Abstractions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(GetUsersQuery).Assembly);
        });
        
        services.Configure<JwtOptions>(
            configuration.GetSection("Jwt"));

        services.Configure<SecurityOptions>(
            configuration.GetSection("Security"));

        services.AddScoped<IPasswordHasher, Argon2PasswordHasher>();
        
        return services;
    }
}