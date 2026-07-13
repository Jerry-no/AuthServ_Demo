using AuthService.Domain.Entity;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _context;
    
    public UserRepository(AuthDbContext context)
    {
        _context = context;
    }
    
    public Task<List<User>> GetUsers()
    {
        throw new NotImplementedException();
    }
    
    public Task<User?> GetUserById(Guid id, CancellationToken ct)
    {
        var user = _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);
        return user;
    }

    public Task<User?> GetUserByEmail(string email,  CancellationToken ct)
    {
        var user = _context.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
        return user;
    }

    public Task<User?> GetUserByUsername(string username, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<int> register(RegisterRequest registerRequest, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<int> adminRegisterForUser(RegisterRequest registerRequest, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}