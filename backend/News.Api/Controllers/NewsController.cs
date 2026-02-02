using Microsoft.AspNetCore.Mvc;
using StockTok.Services.News.Api.Services;

namespace StockTok.Services.News.Api.Controllers;

[ApiController] 
[Route("api/[controller]")] 
public class NewsController : ControllerBase
{
    private readonly NewsService _newsService;
    
    public NewsController(NewsService newsService)
    {
        _newsService = newsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllNewsBySymbolsAsync([FromQuery] string symbols)
    {
        if (string.IsNullOrEmpty(symbols))
            return BadRequest("No symbols provided");

        // Split the string by comma and remove whitespace
        var symbolList = symbols
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

        var articles = await _newsService.GetNewsBySymbolsAsync(symbolList);
        
        return Ok(articles);
    }
}