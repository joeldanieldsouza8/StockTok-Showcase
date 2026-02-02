using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using StockTok.Services.News.Infrastructure.Dtos;
using StockTok.Services.News.Infrastructure.Settings;

namespace StockTok.Services.News.Infrastructure.Clients;

public class NewsApiClient : INewsApiClient
{
    private readonly HttpClient _httpClient;
    private readonly NewsApiSettings _settings;
        
    public NewsApiClient(HttpClient httpClient, IOptions<NewsApiSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
    }
        
    public async Task<List<NewsApiResponseDto.NewsArticleDto>> GetAllNewsBySymbolsAsync(List<string> symbols)
    {        
        var tickers = string.Join(",", symbols);
        
        var requestUri = $"{_settings.BaseUrl}news/all?symbols={tickers}&filter_entities=true&language=en&api_token={_settings.ApiToken}";
            
        var response = await _httpClient.GetAsync(requestUri);

        var options = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
        
        var news = await response.Content.ReadFromJsonAsync<NewsApiResponseDto>(options);
                
        return news?.Data.ToList() ?? new List<NewsApiResponseDto.NewsArticleDto>();
    }
}