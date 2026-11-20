using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.Dtos.Projects;
using PortfolioApi.Entities;

namespace PortfolioApi.Services;

public class ProjectService : IProjectService
{
// DEPENDENCY INJECTION DBCONTEXT
// Deze service ontvangt PortfolioDbContext via de constructor.
// De DbContext wordt geregistreerd in Program.cs met AddDbContext.
// De DbContext staat in Data/PortfolioDbContext.cs.
// De databaseverbinding staat in appsettings.Development.json.
// Via _db kan ik de Projects tabel benaderen met EF Core.
    private readonly PortfolioDbContext _db;

    public ProjectService(PortfolioDbContext db)
    {
        _db = db;
    }
// Haalt alle projecten op via _db.Projects.
// AsNoTracking wordt gebruikt omdat ik deze gegevens alleen lees.
// ToListAsync voert de databasequery asynchroon uit.
// Select(ToDto) zet iedere Project entity om naar een ProjectDto.
// De methode ToDto staat onderaan dit bestand.
// Returntype: Task<List<ProjectDto>>.
    public async Task<List<ProjectDto>> GetAllAsync()
    {
        var projects = await _db.Projects.AsNoTracking().ToListAsync();
        return projects.Select(ToDto).ToList();
    }
// Zoekt een project op basis van het ID.
// FirstOrDefaultAsync geeft het project terug of null als er geen match is.
// p => p.Id == id betekent dat het project ID gelijk moet zijn aan het gevraagde ID.
// Als het project bestaat, zet ToDto de entity om naar een DTO.
// Returntype: Task<ProjectDto?>, omdat het resultaat ook null kan zijn.
    public async Task<ProjectDto?> GetByIdAsync(int id)
    {
        var project = await _db.Projects.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        return project == null ? null : ToDto(project);
    }
// Ontvangt een ProjectCreateDto vanuit ProjectsController.cs.
// De DTO staat in Dtos/Projects/ProjectCreateDto.cs.
// Met new Project maak ik een Project-entity aan.
// De entity staat in Entities/Project.cs.
// De eigenschappen worden overgenomen uit de CreateDto.
// _db.Projects.Add(project) voegt de entity toe aan de DbContext.
// SaveChangesAsync slaat het nieuwe project op in SQL Server.
// ToDto zet de opgeslagen entity om naar ProjectDto.
// Returntype: Task<ProjectDto>.
    public async Task<ProjectDto> CreateAsync(ProjectCreateDto input)
    {
        var project = new Project
        {
            Titel = input.Titel,
            Beschrijving = input.Beschrijving,
            Categorie = input.Categorie,
            GitHubUrl = input.GitHubUrl,
            Datum = input.Datum
        };

        _db.Projects.Add(project);
        await _db.SaveChangesAsync();

        return ToDto(project);
    }
// Ontvangt het project ID en de nieuwe gegevens uit ProjectUpdateDto.
// FindAsync(id) zoekt de bestaande entity op basis van de primaire sleutel.
// Als het project niet bestaat, geeft de methode false terug.
// Anders worden de eigenschappen aangepast.
// SaveChangesAsync slaat de veranderingen op in SQL Server.
// Returntype: Task<bool>.
// ProjectsController.cs zet deze bool om naar 204 of 404.
    public async Task<bool> UpdateAsync(int id, ProjectUpdateDto input)
    {
        var project = await _db.Projects.FindAsync(id);
        if (project == null)
        {
            return false;
        }

        project.Titel = input.Titel;
        project.Beschrijving = input.Beschrijving;
        project.Categorie = input.Categorie;
        project.GitHubUrl = input.GitHubUrl;
        project.Datum = input.Datum;

        await _db.SaveChangesAsync();
        return true;
    }
// Zoekt het project op met FindAsync(id).
// Als het project niet bestaat, geeft de methode false terug.
// Remove(project) markeert de entity voor verwijdering.
// SaveChangesAsync voert de verwijdering uit in SQL Server.
// Returntype: Task<bool>.
// ProjectsController.cs vertaalt dit naar 204 No Content of 404 Not Found.
    public async Task<bool> DeleteAsync(int id)
    {
        var project = await _db.Projects.FindAsync(id);
        if (project == null)
        {
            return false;
        }

        _db.Projects.Remove(project);
        await _db.SaveChangesAsync();
        return true;
    }

// ENTITY NAAR DTO MAPPING
// Deze methode zet een Project entity om naar een ProjectDto.
// De entity staat in Entities/Project.cs.
// De DTO staat in Dtos/Projects/ProjectDto.cs.
// De eigenschappen worden een voor een overgenomen.
// Deze methode wordt gebruikt door GetAllAsync, GetByIdAsync en CreateAsync.
    private static ProjectDto ToDto(Project project)
    {
        return new ProjectDto
        {
            Id = project.Id,
            Titel = project.Titel,
            Beschrijving = project.Beschrijving,
            Categorie = project.Categorie,
            GitHubUrl = project.GitHubUrl,
            Datum = project.Datum
        };
    }
}
