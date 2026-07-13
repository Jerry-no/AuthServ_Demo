using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Persistence;
using AuthService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AuthDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("ECMAnnDb")));

        services.AddScoped<IUserRepository, UserRepository>();
        // services.AddSingleton<IJwtProvider, JwtProvider>();
        // services.AddSingleton<IPasswordHasher, Argon2PasswordHasher>();
        // services.AddSingleton<IClock, SystemClock>();
        // services.AddScoped<IEmailService, EmailService>();
        // services.AddScoped<IStorageService, MinioStorageService>();
        return services;
    }
}