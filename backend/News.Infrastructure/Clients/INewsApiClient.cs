using StockTok.Services.News.Infrastructure.Dtos;

namespace StockTok.Services.News.Infrastructure.Clients;

public interface INewsApiClient
{
    Task<List<NewsApiResponseDto.NewsArticleDto>> GetAllNewsBySymbolsAsync(List<string> symbols);
}