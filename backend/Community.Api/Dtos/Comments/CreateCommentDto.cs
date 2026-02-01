namespace StockTok.Services.Community.Api.Dtos.Comments;

public class CreateCommentDto
{
    public string Body { get; set; } = string.Empty; 
    
    public string Username { get; set; } = string.Empty;  

    
    // Foreign Key
    public Guid PostId { get; set; } 
}