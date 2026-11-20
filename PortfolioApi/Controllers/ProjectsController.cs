using Microsoft.AspNetCore.Mvc;
using PortfolioApi.Dtos.Projects;
using PortfolioApi.Services;

namespace PortfolioApi.Controllers;

// API LAAG
// Deze controller ontvangt HTTP requests voor projecten.
// De controller voert zelf geen databasequeries uit.
// Hiervoor gebruikt hij IProjectService uit de map Services.
// De basis-URL komt uit Properties/launchSettings.json.
// De route hieronder maakt daarvan /api/Projects.
[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
// DEPENDENCY INJECTION
// Deze controller heeft IProjectService nodig.
// De koppeling met ProjectService staat in Program.cs.
// Via de constructor hieronder ontvangt de controller deze service.
// De interface staat in Services/IProjectService.cs.
// De echte methodes staan in Services/ProjectService.cs.
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

// GET /api/Projects
// Haalt alle projecten op via ProjectService.GetAllAsync().
// De service haalt de gegevens op en zet de entities om naar DTO's.
// Returntype: Task<ActionResult<List<ProjectDto>>>.
// Bij succes geeft de controller 200 OK terug.
    [HttpGet]
    public async Task<ActionResult<List<ProjectDto>>> GetAll()
    {
        var projects = await _projectService.GetAllAsync();
        return Ok(projects);
    }

// GET /api/Projects/{id}
// Ontvangt een integer ID vanuit de URL.
// Roept ProjectService.GetByIdAsync(id) aan.
// Returntype: Task<ActionResult<ProjectDto>>.
// Geeft 200 OK met een ProjectDto terug als het project bestaat.
// Geeft 404 Not Found terug als het ID niet bestaat.
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProjectDto>> GetById(int id)
    {
        var project = await _projectService.GetByIdAsync(id);
        if (project == null)
        {
            return NotFound();
        }

        return Ok(project);
    }

// POST /api/Projects
// Ontvangt een ProjectCreateDto vanuit de request body.
// ProjectCreateDto staat in Dtos/Projects/ProjectCreateDto.cs.
// ProjectService.CreateAsync() maakt hiervan een Project entity.
// Daarna wordt de entity via de DbContext opgeslagen in SQL Server.
// Returntype: Task<ActionResult<ProjectDto>>.
// CreatedAtAction geeft 201 Created terug met het nieuwe project
// en een verwijzing naar het GetById-endpoint.
    [HttpPost]
    public async Task<ActionResult<ProjectDto>> Create(ProjectCreateDto input)
    {
        var project = await _projectService.CreateAsync(input);
        return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
    }

// PUT /api/Projects/{id}
// Ontvangt het ID via de URL en de nieuwe gegevens via ProjectUpdateDto.
// De UpdateDto staat in Dtos/Projects/ProjectUpdateDto.cs.
// ProjectService.UpdateAsync() zoekt en wijzigt het project.
// Returntype: Task<IActionResult>.
// Geeft 204 No Content terug bij succes.
// Geeft 404 Not Found terug als het project niet bestaat.
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ProjectUpdateDto input)
    {
        var updated = await _projectService.UpdateAsync(id, input);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

// DELETE /api/Projects/{id}
// Ontvangt het ID van het project dat verwijderd moet worden.
// Roept ProjectService.DeleteAsync(id) aan.
// Returntype: Task<IActionResult>.
// Geeft 204 No Content terug als het verwijderen lukt.
// Geeft 404 Not Found terug als het ID niet bestaat.
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _projectService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
