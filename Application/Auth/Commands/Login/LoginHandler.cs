using AuthService.Application.Mapper;
using AuthService.Domain.Interfaces;

namespace AuthService.Application.Auth.Commands.Login;

public class LoginHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IUserMapper _mapper;

    public LoginHandler(IUserRepository userRepository, IUserMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }
    
}