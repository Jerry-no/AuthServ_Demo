namespace AuthService.Domain.Interfaces;

public interface  IPasswordHasher
{
    string Hash(string password);

    bool Verify(string password, string passwordHash);

    string Algorithm { get; }

    int Version { get; }
}