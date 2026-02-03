using System.ComponentModel.DataAnnotations;

namespace StockTok.Services.Community.Domain.Entities;

public class Post
{
    public Guid Id { get; set; }

    public String Username { get; set; } = string.Empty;

    public String Title { get; set; } = string.Empty;

    public String Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }

    public String Ticker { get; set; }

    // Foreign Keys
    public string AuthorId { get; set; } = null!;
    
    // Navigation property
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}

