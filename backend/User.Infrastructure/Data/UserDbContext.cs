using Microsoft.EntityFrameworkCore;
using StockTok.Services.User.Infrastructure.Data.Configurations;

namespace StockTok.Services.User.Infrastructure.Data;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
    }

    public DbSet<Domain.Entities.User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
    }
}