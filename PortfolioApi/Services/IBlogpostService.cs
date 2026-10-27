using PortfolioApi.Dtos.Blogposts;

namespace PortfolioApi.Services;

// Contract only: implementations and dependency injection are the next step.
public interface IBlogpostService
{
    Task<List<BlogpostDto>> GetAllAsync();
    Task<BlogpostDto?> GetByIdAsync(int id);
    Task<BlogpostDto> CreateAsync(BlogpostCreateDto input);
    Task<bool> UpdateAsync(int id, BlogpostUpdateDto input);
    Task<bool> DeleteAsync(int id);
}
