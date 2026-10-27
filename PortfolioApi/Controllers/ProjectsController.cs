using Microsoft.AspNetCore.Mvc;

namespace PortfolioApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetProjects()
    {
        var projects = new[]
        {
            new
            {
                Id = 1,
                Titel = "Lingo",
                Categorie = "Web"
            },
            new
            {
                Id = 2,
                Titel = "Hotel Simulator",
                Categorie = "Java"
            }
        };

        return Ok(projects);
    }
}