using PortfolioApi.Dtos.Projects;

namespace PortfolioApi.Services;

// INTERFACE / CONTRACT
// Deze interface beschrijft welke methodes ProjectService moet hebben.
// Hier staat alleen WAT de service kan doen, niet HOE.
// ProjectsController.cs gebruikt deze interface.
// ProjectService.cs bevat de uitvoering van deze methodes.
// In Program.cs wordt IProjectService gekoppeld aan ProjectService.
public interface IProjectService
{
    Task<List<ProjectDto>> GetAllAsync();
    Task<ProjectDto?> GetByIdAsync(int id);
    Task<ProjectDto> CreateAsync(ProjectCreateDto input);
    Task<bool> UpdateAsync(int id, ProjectUpdateDto input);
    Task<bool> DeleteAsync(int id);
}
