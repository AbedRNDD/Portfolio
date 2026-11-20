using Microsoft.AspNetCore.Mvc;
using PortfolioApi.Dtos.Blogposts;
using PortfolioApi.Services;

namespace PortfolioApi.Controllers;

// API LAAG VOOR BLOGPOSTS
// Deze controller ontvangt HTTP requests voor blogposts.
// De route is /api/Blogposts.
// De controller gebruikt IBlogpostService voor de acties.
[ApiController]
[Route("api/[controller]")]
public class BlogpostsController : ControllerBase
{
// DEPENDENCY INJECTION
// IBlogpostService wordt via de constructor ontvangen.
// De registratie staat in Program.cs.
// Het contract staat in Services/IBlogpostService.cs.
// De uitvoering staat in Services/BlogpostService.cs.
    private readonly IBlogpostService _blogpostService;

    public BlogpostsController(IBlogpostService blogpostService)
    {
        _blogpostService = blogpostService;
    }
// GetAll:
// GET /api/Blogposts haalt alle blogposts op.
// Returntype: Task<ActionResult<List<BlogpostDto>>>.
    [HttpGet]
    public async Task<ActionResult<List<BlogpostDto>>> GetAll()
    {
        var blogposts = await _blogpostService.GetAllAsync();
        return Ok(blogposts);
    }
// GetById:
// GET /api/Blogposts/{id} haalt één blogpost op.
// Returntype: Task<ActionResult<BlogpostDto>>>.
// Geeft 200 OK of 404 Not Found terug.
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
// Create:
// POST /api/Blogposts ontvangt een BlogpostCreateDto.
// Returntype: Task<ActionResult<BlogpostDto>>>.
// Geeft 201 Created terug.
    [HttpPost]
    public async Task<ActionResult<BlogpostDto>> Create(BlogpostCreateDto input)
    {
        var blogpost = await _blogpostService.CreateAsync(input);
        return CreatedAtAction(nameof(GetById), new { id = blogpost.Id }, blogpost);
    }
// Update:
// PUT /api/Blogposts/{id} gebruikt een BlogpostUpdateDto.
// Returntype: Task<IActionResult>.
// Geeft 204 No Content of 404 Not Found terug.
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
// Delete:
// DELETE /api/Blogposts/{id} verwijdert een blogpost.
// Returntype: Task<IActionResult>.
// Geeft 204 No Content of 404 Not Found terug.
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
