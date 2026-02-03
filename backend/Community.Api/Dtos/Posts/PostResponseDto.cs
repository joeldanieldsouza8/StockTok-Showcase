using StockTok.Services.Community.Api.Dtos.Comments;

namespace StockTok.Services.Community.Api.Dtos.Posts;

public class PostResponseDto
{
    public Guid Id { get; set; }

    public string Username { get; set; } = string.Empty;
    
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public string Ticker { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    // Foreign Keys
    public string AuthorId { get; set; } = string.Empty;

    public List<CommentResponseDto> Comments { get; set; } = new();
}   