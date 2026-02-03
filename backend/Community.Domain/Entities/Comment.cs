namespace StockTok.Services.Community.Domain.Entities;

public class Comment
{
    public Guid Id { get; set; } 

    public string Body { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } 
    
    public DateTime UpdatedAt { get; set; }

    // Foreign Keys
    public Guid PostId { get; set; }
    public string AuthorId { get; set; } = null!;

    // Navigation Property
    public Post Post { get; set; } = null!;
}