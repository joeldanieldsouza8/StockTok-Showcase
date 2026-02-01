using Microsoft.EntityFrameworkCore;
using StockTok.Services.Community.Api.Dtos.Comments;
using StockTok.Services.Community.Api.Dtos.Posts;
using StockTok.Services.Community.Domain.Entities;
using StockTok.Services.Community.Infrastructure.Data;

namespace StockTok.Services.Community.Api.Services;

public class PostService
{
    private readonly CommunityDbContext _context;
    
    public PostService(CommunityDbContext context)
    {
        _context = context;
    }

    public async Task<PostResponseDto> CreatePostAsync(CreatePostDto createPostDto, string authorId)
    {
        // Create the record for the new post
        var newPost = new Post
        {
            Id = Guid.NewGuid(), 
            Title = createPostDto.Title,
            Description = createPostDto.Description,
            Ticker = createPostDto.Ticker,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            
            // Foreign Key
            AuthorId = authorId,
        };

        // Save the post to the database
        _context.Posts.Add(newPost);
        await _context.SaveChangesAsync();
        
        // Map the post from the entity type to the dto type
        var newPostDto = MapToDto(newPost);
        
        return newPostDto;
    }

    public async Task<List<PostResponseDto>> GetAllPostsBySymbolAsync(string ticker)
    {
        // Query the database for all the posts by the ticker
        var posts = await _context.Posts
            .AsNoTracking()
            .Where(x => x.Ticker == ticker)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
        
        return posts
            .Select(MapToDto)
            .ToList();
    }

    public async Task<PostResponseDto> GetPostByIdAsync(Guid id)
    {
        // Query the database for the post
        var post = await _context.Posts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        return post != null ? MapToDto(post) : new PostResponseDto();
    }

    public async Task<PostResponseDto> UpdatePostByIdAsync(string id, UpdatePostDto updateDto, string authorId)
    {
        // Query the database for the post
        var post = await _context.Posts
            .FindAsync(id);
        
        if (post == null)
        {
            return new PostResponseDto();
        }

        if (post.AuthorId != authorId)
        {
            throw new UnauthorizedAccessException("You do not own this post.");
        }

        post.Title = updateDto.Title;
        post.Description = updateDto.Description;
        post.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        
        var updatePostDto =  MapToDto(post);
        
        return updatePostDto;
    }

    public async Task<bool> DeletePostByIdAsync(string id, string authorId)
    {
        // Query the database for the post
        var post = await _context.Posts
            .FindAsync(id);
        
        if (post == null)
        {
            return false;
        }

        if (post.AuthorId != authorId)
        {
            throw new UnauthorizedAccessException("You do not own this post.");
        }
        
        var ticker = post.Ticker; 

        // Remove the post from the database
        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();
        
        return true;
    }

    private static PostResponseDto MapToDto(Post post)
    {
        return new PostResponseDto
        {
            Id = post.Id,
            Title = post.Title,
            Description = post.Description,
            Ticker = post.Ticker,
            AuthorId = post.AuthorId,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        };
    }
}