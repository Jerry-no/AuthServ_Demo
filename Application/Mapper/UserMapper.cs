using AuthService.Application.Auth.Responses.Users;
using AuthService.Domain.Entity;
using Riok.Mapperly.Abstractions;

namespace AuthService.Application.Mapper;

[Mapper(
    RequiredMappingStrategy = RequiredMappingStrategy.Target
)]
public partial class UserMapper : IUserMapper
{
    public partial UserResponse ToResponse(User entity);
}