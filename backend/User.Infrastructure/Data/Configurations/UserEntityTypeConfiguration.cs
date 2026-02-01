using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StockTok.Services.User.Infrastructure.Data.Configurations;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<Domain.Entities.User>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.User> builder)
    {
        builder.ToTable("Users");
        
        // Configure the primary key
        builder.HasKey(x => x.Id);
        
        builder.Property(u => u.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);
    }
}