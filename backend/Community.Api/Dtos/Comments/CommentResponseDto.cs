namespace StockTok.Services.Community.Api.Dtos.Comments;

public class CommentResponseDto
{
    public Guid Id { get; set; } 
    
    public string Body { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    
    // Foreign Keys
    public string AuthorId { get; set; } = null!;
    
    public Guid PostId { get; set; } 
}