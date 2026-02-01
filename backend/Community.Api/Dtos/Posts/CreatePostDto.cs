namespace StockTok.Services.Community.Api.Dtos.Posts;

public class CreatePostDto
{

    public string Id { get; set; } = string.Empty;

    public string Username {  get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public string Ticker { get; set; } = string.Empty;
}