using AuthService.Application.Auth.Responses.Users;
using AuthService.Application.Mapper;
using AuthService.Common.Abstractions.Messaging;
using AuthService.Common.ErrorCodes;
using AuthService.Common.Exceptions;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Application.Auth.Queries.Users.GetUserByMail;

public class GetUserByMailHandler : IQueryHandler<GetUserByMailQuery, UserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserMapper _mapper;
    
    public GetUserByMailHandler(IUserRepository repository, IUserMapper mapper)
    {
        _userRepository = repository;
        _mapper = mapper;
    }
    
    public async Task<UserResponse> Handle(GetUserByMailQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByEmail(request.Email, cancellationToken);
        if (user is null) throw new AppException(new ErrorCode(1,"User not found!"));
        return _mapper.ToResponse(user);
    }
}