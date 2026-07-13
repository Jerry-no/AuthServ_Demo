using AuthService.Domain.Entity;
using Microsoft.AspNetCore.Identity.Data;

namespace AuthService.Domain.Interfaces;

public interface IUserRepository
{
     Task<List<User>> GetUsers();
     Task<User?> GetUserById(Guid id, CancellationToken ct);
     Task<User?> GetUserByEmail(string email,  CancellationToken ct);
     
     Task<User?> GetUserByUsername(string username, CancellationToken ct);
     
     Task<int> register(RegisterRequest registerRequest, CancellationToken ct);
     Task<int> adminRegisterForUser(RegisterRequest registerRequest, CancellationToken ct);
}