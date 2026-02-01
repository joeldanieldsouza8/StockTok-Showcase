using System.Reflection;
using Microsoft.EntityFrameworkCore;
using StockTok.Services.News.Domain.Entities;

namespace StockTok.Services.News.Infrastructure.Data;

public class NewsDbContext : DbContext
{
    public NewsDbContext(DbContextOptions<NewsDbContext> options) : base(options)
    {
    }
    
    public DbSet<NewsArticle> NewsArticles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("News");

        modelBuilder.Entity<NewsArticle.NewsArticleEntity>()
        .ToTable("NewsArticleEntities");

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}