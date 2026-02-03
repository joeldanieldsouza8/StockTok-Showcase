using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StockTok.Services.News.Api.Services;
using StockTok.Services.News.Infrastructure.Clients;
using StockTok.Services.News.Infrastructure.Data;
using StockTok.Services.News.Infrastructure.Settings;

namespace StockTok.Services.News.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ConfigureServices(builder);

        var app = builder.Build();

        ConfigureMiddleware(app);

        app.Run();
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}/";
            options.Audience = builder.Configuration["Auth0:Audience"];
        });
        
        services.AddDbContext<NewsDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("NewsDatabase")));

        services.AddOptions<NewsApiSettings>()
            .BindConfiguration("NewsApiSettings")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddHttpClient<INewsApiClient, NewsApiClient>((serviceProvider, client) =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<NewsApiSettings>>().Value;
            client.BaseAddress = new Uri(settings.BaseUrl);
        });

        services.AddScoped<NewsService>();

        services.AddControllers()
            .AddJsonOptions(options =>
            {
                // Prevents loops when serializing complex objects
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.WriteIndented = true;
            });

        services.AddEndpointsApiExplorer();
    }

    private static void ConfigureMiddleware(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            // app.UseSwagger();
            // app.UseSwaggerUI();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
    }
}