namespace AuthService.Common.Models;

public abstract class EntityBase
{
    public Guid Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    
    public DateTimeOffset UpdatedAt { get; set; }

    public string? Noted { get; set; }

    public bool IsDeleted { get; set; }
}