using PortfolioApi.Dtos.Projects;

namespace PortfolioApi.Services;

// Contract only: implementations and dependency injection are the next step.
public interface IProjectService
{
    Task<List<ProjectDto>> GetAllAsync();
    Task<ProjectDto?> GetByIdAsync(int id);
    Task<ProjectDto> CreateAsync(ProjectCreateDto input);
    Task<bool> UpdateAsync(int id, ProjectUpdateDto input);
    Task<bool> DeleteAsync(int id);
}
