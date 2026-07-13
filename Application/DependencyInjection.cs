using AuthService.Application.Mapper;

namespace AuthService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddSingleton<IUserMapper, UserMapper>();
        return services;
    }
}