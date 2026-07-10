using AuthService.Application.Auth.Responses.Users;
using AuthService.Domain.Entity;

namespace AuthService.Application.Mapper;

public interface IUserMapper
{
    UserResponse ToResponse(User entity);
}