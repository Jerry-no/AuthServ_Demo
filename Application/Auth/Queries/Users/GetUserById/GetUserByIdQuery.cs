using AuthService.Application.Auth.Responses.Users;
using AuthService.Common.Abstractions.Messaging;

namespace AuthService.Application.Auth.Queries.Users.GetUserById;

public sealed record GetUserByIdQuery(
    Guid Id
) : IQuery<UserResponse>;