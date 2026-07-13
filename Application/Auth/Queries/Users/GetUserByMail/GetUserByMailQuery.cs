using AuthService.Application.Auth.Responses.Users;
using AuthService.Common.Abstractions.Messaging;

namespace AuthService.Application.Auth.Queries.Users.GetUserByMail;

public sealed record GetUserByMailQuery(
    string Email
) : IQuery<UserResponse>;