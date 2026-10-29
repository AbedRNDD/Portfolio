using Microsoft.AspNetCore.Mvc;
using PortfolioApi.Dtos.Blogposts;
using PortfolioApi.Services;

namespace PortfolioApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlogpostsController : ControllerBase
{
    private readonly IBlogpostService _blogpostService;

    public BlogpostsController(IBlogpostService blogpostService)
    {
        _blogpostService = blogpostService;
    }

    [HttpGet]
    public async Task<ActionResult<List<BlogpostDto>>> GetAll()
    {
        var blogposts = await _blogpostService.GetAllAsync();
        return Ok(blogposts);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BlogpostDto>> GetById(int id)
    {
        var blogpost = await _blogpostService.GetByIdAsync(id);
        if (blogpost == null)
        {
            return NotFound();
        }

        return Ok(blogpost);
    }

    [HttpPost]
    public async Task<ActionResult<BlogpostDto>> Create(BlogpostCreateDto input)
    {
        var blogpost = await _blogpostService.CreateAsync(input);
        return CreatedAtAction(nameof(GetById), new { id = blogpost.Id }, blogpost);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, BlogpostUpdateDto input)
    {
        var updated = await _blogpostService.UpdateAsync(id, input);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _blogpostService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
