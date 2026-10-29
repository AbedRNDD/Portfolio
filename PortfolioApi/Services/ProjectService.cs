using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.Dtos.Projects;
using PortfolioApi.Entities;

namespace PortfolioApi.Services;

public class ProjectService : IProjectService
{
    private readonly PortfolioDbContext _db;

    public ProjectService(PortfolioDbContext db)
    {
        _db = db;
    }

    public async Task<List<ProjectDto>> GetAllAsync()
    {
        var projects = await _db.Projects.AsNoTracking().ToListAsync();
        return projects.Select(ToDto).ToList();
    }

    public async Task<ProjectDto?> GetByIdAsync(int id)
    {
        var project = await _db.Projects.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        return project == null ? null : ToDto(project);
    }

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
