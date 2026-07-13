using AuthService.Application.Mapper;
using AuthService.Domain.Interfaces;

namespace AuthService.Application.Auth.Commands.AdminRegister;

public class AdminRegisterHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IUserMapper _userMapper;

    public AdminRegisterHandler(IUserRepository userRepository, IUserMapper userMapper)
    {
        _userRepository = userRepository;
        _userMapper = userMapper;
    }
    
}