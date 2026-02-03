using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using StockTok.Services.News.Api.Services;
using StockTok.Services.News.Domain.Entities;
using StockTok.Services.News.Infrastructure.Clients;
using StockTok.Services.News.Infrastructure.Data;

namespace StockTok.Services.News.Tests;

public class NewsServiceTests
{
    private readonly Mock<INewsApiClient> _mockNewsApiClient;
    private readonly NewsDbContext _dbContext;
    private readonly NewsService _newsService; 

    public NewsServiceTests()
    {
        // Create the in-memory database
        var options = new DbContextOptionsBuilder<NewsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique name per test
            .Options;
        
        _dbContext = new NewsDbContext(options);

        // Create the fake API Client
        _mockNewsApiClient = new Mock<INewsApiClient>();

        // Initialise the service with our fakes
        _newsService = new NewsService(_dbContext, _mockNewsApiClient.Object);
    }

    [Fact]
    public async Task GetNewsBySymbolsAsync_ShouldReturnCachedNews_WhenNewsIsFresh()
    {
        var symbol = "NVDA";
        var freshDate = DateTime.UtcNow.AddHours(-1); // 1 hour ago (Fresh is < 6 hours)

        // Seed the fake database with a fresh article
        var existingArticle = new NewsArticle
        {
            Uuid = Guid.NewGuid(),
            Title = "Nvidia is doing great",
            PublishedAt = freshDate,
            NewsArticleEntities = new List<NewsArticle.NewsArticleEntity>
            {
                new() { Symbol = symbol, Name = "Nvidia" }
            }
        };

        await _dbContext.NewsArticles.AddAsync(existingArticle);
        await _dbContext.SaveChangesAsync();

        var result = await _newsService.GetNewsBySymbolsAsync(new List<string> { symbol });
        
        // Assert the results
        // We should get 1 article back
        result.Should().HaveCount(1);
        result.First().Title.Should().Be("Nvidia is doing great");

        // Ensure the External API was never called because we had fresh data
        _mockNewsApiClient.Verify(
            x => x.GetAllNewsBySymbolsAsync(It.IsAny<List<string>>()), 
            Times.Never);
    }
}