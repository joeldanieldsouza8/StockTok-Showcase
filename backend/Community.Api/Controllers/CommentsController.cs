using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockTok.Services.Community.Api.Dtos.Comments;
using StockTok.Services.Community.Api.Services;

namespace StockTok.Services.Community.Api.Controllers;

[ApiController]
[Route("api/community/[controller]")]
[Authorize]
public class CommentsController : ControllerBase
{
    private readonly CommentService _commentService;

    public CommentsController(CommentService commentService)
    {
        _commentService = commentService;
    }
    
    // GET: api/comments/post/{postId}
    [HttpGet("post/{postId:guid}", Name = "GetAllCommentsByPostIdAsync")]
    public async Task<IActionResult> GetAllCommentsByPostIdAsync(Guid postId)
    {
        var comments = await _commentService.GetAllCommentsByPostIdAsync(postId);
        
        return Ok(comments);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCommentAsync([FromBody] CreateCommentDto createCommentDto)
    {
        // Get the user id from the JWT
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        // Guard clause
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }
        
        var newComment = await _commentService.CreateCommentAsync(createCommentDto, userId);
        
        return CreatedAtRoute(nameof(GetAllCommentsByPostIdAsync), new { id = newComment.Id }, newComment);
    }

    // PUT: api/comments/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCommentByIdAsync(Guid id, [FromBody] UpdateCommentDto updateDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }
        
        var updatedComment = await _commentService.UpdateCommentByIdAsync(id, updateDto, userId);
        
        if (string.IsNullOrEmpty(updatedComment.Id.ToString()))
        {
            return NotFound();
        }
        
        return Ok(updatedComment);
    }

    // DELETE: api/comments/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCommentByIdAsync(string id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }
        
        var success = await _commentService.DeleteCommentByIdAsync(id, userId);
        
        if (!success)
        {
            return NotFound();
        }
        
        return NoContent();
    }
}