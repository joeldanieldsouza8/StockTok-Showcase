using Microsoft.EntityFrameworkCore;
using StockTok.Services.News.Domain.Entities;
using StockTok.Services.News.Infrastructure.Clients;
using StockTok.Services.News.Infrastructure.Data;
using StockTok.Services.News.Infrastructure.Dtos;

namespace StockTok.Services.News.Api.Services;

public class NewsService
{
    private readonly NewsDbContext _context;
    private readonly NewsApiClient _newsApiClient;

    public NewsService(NewsDbContext context, NewsApiClient newsApiClient)
    {
        _context = context;
        _newsApiClient = newsApiClient;
    }

    public async Task<List<NewsArticle>> GetNewsBySymbolsAsync(List<string> symbols)
    {
        // Ensure the symbols are uppercase and unique to avoid duplicates
        var distinctSymbols = symbols
            .Select(s => s.Trim().ToUpper())
            .Distinct()
            .ToHashSet();
        
        // Check if there are no symbols in the list
        if (distinctSymbols.Count == 0)
        {
            return new List<NewsArticle>();
        }
        
        // Calculate the cutoff time to determine if stored news is still considered "fresh" (6-hour cache)
        var freshCutoffTime = DateTime.UtcNow.AddHours(-6);

        // // Query the database for all news articles related to the requested symbols
        // var dbArticles = await _context.NewsArticles
        //     .Include(a => a.NewsArticleEntities)
        //     .Where(a => a.NewsArticleEntities.Any(e => distinctSymbols.Contains(e.Symbol)))
        //     .OrderByDescending(a => a.PublishedAt)
        //     .ToListAsync();
        //
        // // Check which news articles of symbols from the requested symbols list has at least one article for it that is newer than the cutoff
        // var freshSymbols = dbArticles
        //     .Where(a => a.PublishedAt >= freshCutoffTime)
        //     .SelectMany(a => a.NewsArticleEntities)
        //     .Where(e => distinctSymbols.Contains(e.Symbol)) 
        //     .Select(e => e.Symbol)
        //     .Distinct()
        //     .ToList();
        
        var freshSymbols = await _context.NewsArticles
            .Where(a => a.PublishedAt >= freshCutoffTime) 
            .SelectMany(a => a.NewsArticleEntities)
            .Where(e => distinctSymbols.Contains(e.Symbol))
            .Select(e => e.Symbol)
            .Distinct()
            .ToListAsync();
        
        var missingSymbols = distinctSymbols.Except(freshSymbols).ToList();
        
        // Initialise a list for only if we have news articles of symbols that we need to fetch

        if (missingSymbols.Count > 0)
        {
            // Query the MarketAux API only for the news articles of the symbols we need to query that are either stale or don't yet exist in the database
            var apiDtos = await _newsApiClient.GetAllNewsBySymbolsAsync(missingSymbols);

            // // Filter out duplicates which are news articles we might already have from a previous partial fetch
            // var existingUuids = dbArticles.Select(a => a.Uuid).ToHashSet();
            //
            // var uniqueDtos = apiDtos
            //     .Where(dto => !existingUuids.Contains(dto.Uuid))
            //     .ToList();
            
            var newUuids = apiDtos.Select(d => d.Uuid).ToList();
            var existingUuids = await _context.NewsArticles
                .Where(a => newUuids.Contains(a.Uuid))
                .Select(a => a.Uuid)
                .ToHashSetAsync();

            var uniqueDtos = apiDtos
                .Where(dto => !existingUuids.Contains(dto.Uuid))
                .ToList();

            var newArticles = uniqueDtos.Select(NewsArticleDtoToEntity).ToList();

            // If there are new news articles that were queried from the MarketAux API, then save it to the database
            if (newArticles.Count > 0)
            {
                await _context.NewsArticles.AddRangeAsync(newArticles);
                await _context.SaveChangesAsync();
            }
        }
        
        // Combine the existing news articles of the requested symbols from the database, and the newly fetched news articles of the requested symbols
        var allArticles = await _context.NewsArticles
            .Include(a => a.NewsArticleEntities)
            .Where(a => a.PublishedAt >= freshCutoffTime) 
            .Where(a => a.NewsArticleEntities.Any(e => distinctSymbols.Contains(e.Symbol)))
            .OrderByDescending(a => a.PublishedAt)
            .ToListAsync();

        return allArticles;
    }

    private static NewsArticle NewsArticleDtoToEntity(NewsApiResponseDto.NewsArticleDto newsArticleDto)
    {
        var newArticle = new NewsArticle
        {
            Uuid = newsArticleDto.Uuid,
            Title = newsArticleDto.Title,
            Description = newsArticleDto.Description,
            Url = newsArticleDto.Url,
            Language = newsArticleDto.Language,
            PublishedAt = newsArticleDto.PublishedAt.ToUniversalTime(), // Ensure UTC
            NewsArticleEntities = newsArticleDto.Entities.Select(e => new NewsArticle.NewsArticleEntity
            {
                Symbol = e.Symbol,
                Name = e.Name,
                Country = e.Country,
                Industry = e.Industry,
            }).ToList()
        };

        return newArticle;
    }
}