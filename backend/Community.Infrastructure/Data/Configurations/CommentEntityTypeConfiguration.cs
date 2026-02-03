using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockTok.Services.Community.Domain.Entities;

namespace StockTok.Services.Community.Infrastructure.Data.Configurations;

public class CommentEntityTypeConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(p => p.Id);
    }
}