using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using StockTok.Services.User.Infrastructure.Data;
using StockTok.Services.User.Api.Services;

namespace StockTok.Services.User.Api;

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
        
        services.AddDbContext<UserDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("UserDatabase")));
        
        services.AddScoped<UserService>();
        // services.AddScoped<WatchlistService>(); // Uncomment this when you migrate the WatchlistService
        
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}/";
            options.Audience = builder.Configuration["Auth0:Audience"];
        });

        services.AddAuthorization();
        
        services.AddControllers();
        services.AddEndpointsApiExplorer();
    }

    private static void ConfigureMiddleware(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            // app.UseSwagger();
            // app.UseSwaggerUI();
        }
        
        // app.UseHttpsRedirection(); // Keep commented out for internal Docker networking
        
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapControllers();
    }
}