using AuthService.Application.Auth.Responses.Users;
using AuthService.Common.Abstractions.Messaging;
using AuthService.Common.Constants;
using AuthService.Common.Dtos;

namespace AuthService.Application.Auth.Queries.Users.GetUsers;

public sealed class GetUsersQuery : PageRequest, IQuery<PageResponse<UserResponse>>
{
    public bool? Enabled { get; init; }

    public UserStatus? Status { get; init; }

    public bool? MfaEnabled { get; init; }

    public bool? LockedOnly { get; init; }
}