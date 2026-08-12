using System;
using System.Security.Claims;
using System.Threading.Tasks;
using ass01.BusinessLogic.DTOs.NewsArticle;
using ass01.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using System.Collections.Generic;

namespace ass01.Presentation.Controllers;

[ApiController]
[Route("api/news")]
public class NewsArticleController : ControllerBase
{
    private readonly INewsArticleService _newsService;

    public NewsArticleController(INewsArticleService newsService)
    {
        _newsService = newsService;
    }

    [HttpGet]
    [AllowAnonymous]
    [EnableQuery]
    public async Task<IActionResult> GetNewsArticles(
        [FromQuery] string? keyword = null,
        [FromQuery] short? categoryId = null,
        [FromQuery] string? tagName = null,
        [FromQuery] short? createdById = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        bool isStaff = User.Identity?.IsAuthenticated == true && User.IsInRole("Staff");
        var articles = await _newsService.GetNewsArticlesAsync(isStaff, keyword, categoryId, tagName, createdById, startDate, endDate);
        return Ok(articles);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetNewsArticleById(string id)
    {
        bool isStaff = User.Identity?.IsAuthenticated == true && User.IsInRole("Staff");
        var article = await _newsService.GetNewsArticleByIdAsync(id, isStaff);
        if (article == null) return NotFound(new { message = "NewsArticle not found." });
        return Ok(article);
    }

    [HttpPost]
    [Authorize(Roles = "Staff")]
    public async Task<IActionResult> CreateNewsArticle([FromBody] CreateNewsArticleRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!short.TryParse(currentUserIdStr, out var currentUserId))
            return Unauthorized();

        try
        {
            var created = await _newsService.CreateNewsArticleAsync(request, currentUserId);
            return CreatedAtAction(nameof(GetNewsArticleById), new { id = created.NewsArticleId }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Staff")]
    public async Task<IActionResult> UpdateNewsArticle(string id, [FromBody] UpdateNewsArticleRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!short.TryParse(currentUserIdStr, out var currentUserId))
            return Unauthorized();

        try
        {
            await _newsService.UpdateNewsArticleAsync(id, request, currentUserId);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "NewsArticle not found." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Staff")]
    public async Task<IActionResult> DeleteNewsArticle(string id)
    {
        try
        {
            await _newsService.DeleteNewsArticleAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "NewsArticle not found." });
        }
    }

    [HttpPost("{id}/duplicate")]
    [Authorize(Roles = "Staff")]
    public async Task<IActionResult> DuplicateNewsArticle(string id)
    {
        var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!short.TryParse(currentUserIdStr, out var currentUserId))
            return Unauthorized();

        try
        {
            var duplicated = await _newsService.DuplicateNewsArticleAsync(id, currentUserId);
            return CreatedAtAction(nameof(GetNewsArticleById), new { id = duplicated.NewsArticleId }, duplicated);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Original NewsArticle not found." });
        }
    }

    [HttpGet("{id}/related")]
    [AllowAnonymous]
    public async Task<IActionResult> GetRelatedNewsArticles(string id)
    {
        try
        {
            var related = await _newsService.GetRelatedNewsArticlesAsync(id);
            return Ok(related);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "NewsArticle not found." });
        }
    }

    [HttpGet("my-history")]
    [Authorize(Roles = "Staff")]
    public async Task<IActionResult> GetMyHistory()
    {
        var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!short.TryParse(currentUserIdStr, out var currentUserId))
            return Unauthorized();

        // Pass createdById = currentUserId
        // Other params are null to get all history
        var articles = await _newsService.GetNewsArticlesAsync(
            isStaff: true, 
            keyword: null, 
            categoryId: null, 
            tagName: null, 
            createdById: currentUserId, 
            startDate: null, 
            endDate: null);

        return Ok(articles);
    }
}
