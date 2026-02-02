using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockTok.Services.Community.Api.Dtos.Posts;
using StockTok.Services.Community.Api.Services;

namespace StockTok.Services.Community.Api.Controllers;

[ApiController]
[Route("api/community/[controller]")]
[Authorize]
public class PostsController : ControllerBase
{
    private readonly PostService _postService;

    public PostsController(PostService postService)
    {
        _postService = postService;
    }

    // GET: api/posts?ticker=TSLA
    [HttpGet]
    public async Task<IActionResult> GetAllPostsBySymbolAsync([FromQuery] string ticker)
    {
        var posts = await _postService.GetAllPostsBySymbolAsync(ticker);
        
        return Ok(posts);
    }

    // GET: api/posts/{id}
    [HttpGet("{id:guid}", Name = nameof(GetPostByIdAsync))] 
    public async Task<IActionResult> GetPostByIdAsync(Guid id)
    {
        var post = await _postService.GetPostByIdAsync(id);
        
        // if (string.IsNullOrEmpty(post.Id))
        // {
        //     return NotFound();
        // }
        
        return Ok(post);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePostAsync([FromBody] CreatePostDto createPostDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }
        
        var newPost = await _postService.CreatePostAsync(createPostDto, userId);
        
        return CreatedAtRoute(nameof(GetPostByIdAsync), new { id = newPost.Id }, newPost);
    }

    // PUT: api/posts/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePostByIdAsync(string id, [FromBody] UpdatePostDto updateDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }
        
        var updatedPost = await _postService.UpdatePostByIdAsync(id, updateDto, userId);
        
        if (string.IsNullOrEmpty(updatedPost.Id.ToString()))
        {
            return NotFound();
        }
        
        return Ok(updatedPost);
    }

    // DELETE: api/posts/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePostByIdAsync(string id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }
        
        var success = await _postService.DeletePostByIdAsync(id, userId);
        
        if (!success)
        {
            return NotFound();
        }
        
        return NoContent();
    }
}
