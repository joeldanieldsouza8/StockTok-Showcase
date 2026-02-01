using StockTok.Services.User.Infrastructure.Data;

namespace StockTok.Services.User.Api.Services;

public class UserService
{
    private readonly UserDbContext _context;

    public UserService(UserDbContext context)
    {
        _context = context;
    }
}