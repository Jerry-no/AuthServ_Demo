using AuthService.Application.Auth.Responses.Users;
using AuthService.Application.Mapper;
using AuthService.Common.Abstractions.Messaging;
using AuthService.Common.ErrorCodes;
using AuthService.Common.Exceptions;
using AuthService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Application.Auth.Queries.Users.GetUserById;

public class GetUserByIdHandler : IQueryHandler<GetUserByIdQuery, UserResponse>
{
    private readonly AuthDbContext _dbContext;
    private readonly IUserMapper _mapper;
    
    public GetUserByIdHandler(AuthDbContext dbContext, IUserMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    
    public async Task<UserResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Users.AsNoTracking().AsQueryable();
        var user = await query.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (user is null) throw new AppException(new ErrorCode(1,"UserNotFound"));
        return _mapper.ToResponse(user);
    }
}