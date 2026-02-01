using Microsoft.EntityFrameworkCore;
using StockTok.Services.Community.Domain.Entities;
using StockTok.Services.Community.Infrastructure.Data.Configurations;

namespace StockTok.Services.Community.Infrastructure.Data
{
    public class CommunityDbContext : DbContext
    {
        public CommunityDbContext(DbContextOptions options) : base(options)
        {
        }

        protected CommunityDbContext()
        {
        }

        public DbSet<Post> Posts { get; set; }

        public DbSet<Comment> Comments { get; set; }

        // validation

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            var postEntity = modelBuilder.Entity<Post>();

            postEntity
                .Property(post => post.Id)
                .IsRequired();

            postEntity
                .Property(post => post.Title)
                .HasMaxLength(100)
                .IsRequired();

            postEntity
                .Property(post => post.Description)
                .HasMaxLength(500)
                .IsRequired();

            postEntity
                .Property(post => post.CreatedAt)
                .HasDefaultValueSql("NOW() AT TIME ZONE 'utc'");

            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new PostEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CommentEntityTypeConfiguration());
        }
    }
}
