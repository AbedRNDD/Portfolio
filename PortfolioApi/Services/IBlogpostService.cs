using PortfolioApi.Dtos.Blogposts;

namespace PortfolioApi.Services;

// INTERFACE / CONTRACT VOOR BLOGPOSTS
// Deze interface beschrijft de vijf methodes voor blogposts.
// BlogpostsController.cs gebruikt IBlogpostService.
// BlogpostService.cs voert de methodes uit.
// De koppeling tussen interface en service staat in Program.cs.
public interface IBlogpostService
{
    Task<List<BlogpostDto>> GetAllAsync();
    Task<BlogpostDto?> GetByIdAsync(int id);
    Task<BlogpostDto> CreateAsync(BlogpostCreateDto input);
    Task<bool> UpdateAsync(int id, BlogpostUpdateDto input);
    Task<bool> DeleteAsync(int id);
}
