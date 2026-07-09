using AuthService.Application.Auth.Responses.Users;
using AuthService.Common.Abstractions.Messaging;
using AuthService.Common.Dtos;
using AuthService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Application.Auth.Queries.Users.GetUsers;

public sealed class GetUsersHandler
    : IQueryHandler<GetUsersQuery, PageResponse<UserResponse>>
{
    private readonly AuthDbContext _dbContext;

    public GetUsersHandler(AuthDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PageResponse<UserResponse>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Users
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = request.Keyword.Trim();

            query = query.Where(x =>
                x.Username.Contains(keyword) ||
                (x.Email != null && x.Email.Contains(keyword)) ||
                (x.PhoneNumber != null && x.PhoneNumber.Contains(keyword)) ||
                (x.FullName != null && x.FullName.Contains(keyword)));
        }

        if (request.Enabled.HasValue)
        {
            query = query.Where(x => x.Enabled == request.Enabled.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status.Value);
        }

        if (request.MfaEnabled.HasValue)
        {
            query = query.Where(x => x.MfaEnabled == request.MfaEnabled.Value);
        }

        if (request.LockedOnly == true)
        {
            var now = DateTimeOffset.UtcNow;

            query = query.Where(x =>
                x.LockedUntil.HasValue &&
                x.LockedUntil > now);
        }

        query = request.SortBy?.Trim().ToLowerInvariant() switch
        {
            "username" => IsAscending(request)
                ? query.OrderBy(x => x.Username)
                : query.OrderByDescending(x => x.Username),

            "email" => IsAscending(request)
                ? query.OrderBy(x => x.Email)
                : query.OrderByDescending(x => x.Email),

            "fullname" => IsAscending(request)
                ? query.OrderBy(x => x.FullName)
                : query.OrderByDescending(x => x.FullName),

            "status" => IsAscending(request)
                ? query.OrderBy(x => x.Status)
                : query.OrderByDescending(x => x.Status),

            "lastloginat" => IsAscending(request)
                ? query.OrderBy(x => x.LastLoginAt)
                : query.OrderByDescending(x => x.LastLoginAt),

            _ => query.OrderBy(x => x.Username)
        };

        var totalRecords = await query.LongCountAsync(cancellationToken);

        var items = await query
            .Skip(request.Skip)
            .Take(request.PageSize)
            .Select(x => new UserResponse
            {
                Id = x.Id,
                Username = x.Username,
                FullName = x.FullName,
                Email = x.Email,
                Enabled = x.Enabled,
                Status = x.Status,
                MfaEnabled = x.MfaEnabled,
                LastLoginAt = x.LastLoginAt
            })
            .ToListAsync(cancellationToken);

        return PageResponse<UserResponse>.Create(
            items,
            request.PageNumber,
            request.PageSize,
            totalRecords);
    }

    private static bool IsAscending(PageRequest request)
    {
        return string.Equals(
            request.SortDirection,
            "asc",
            StringComparison.OrdinalIgnoreCase);
    }
}