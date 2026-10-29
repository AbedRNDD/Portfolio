using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.Dtos.Blogposts;
using PortfolioApi.Entities;

namespace PortfolioApi.Services;

public class BlogpostService : IBlogpostService
{
    private readonly PortfolioDbContext _db;

    public BlogpostService(PortfolioDbContext db)
    {
        _db = db;
    }

    public async Task<List<BlogpostDto>> GetAllAsync()
    {
        var blogposts = await _db.Blogposts.AsNoTracking().ToListAsync();
        return blogposts.Select(ToDto).ToList();
    }

    public async Task<BlogpostDto?> GetByIdAsync(int id)
    {
        var blogpost = await _db.Blogposts.AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id);

        return blogpost == null ? null : ToDto(blogpost);
    }

    public async Task<BlogpostDto> CreateAsync(BlogpostCreateDto input)
    {
        var blogpost = new Blogpost
        {
            Titel = input.Titel,
            Inhoud = input.Inhoud,
            Publicatiedatum = input.Publicatiedatum
        };

        _db.Blogposts.Add(blogpost);
        await _db.SaveChangesAsync();

        return ToDto(blogpost);
    }

    public async Task<bool> UpdateAsync(int id, BlogpostUpdateDto input)
    {
        var blogpost = await _db.Blogposts.FindAsync(id);
        if (blogpost == null)
        {
            return false;
        }

        blogpost.Titel = input.Titel;
        blogpost.Inhoud = input.Inhoud;
        blogpost.Publicatiedatum = input.Publicatiedatum;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var blogpost = await _db.Blogposts.FindAsync(id);
        if (blogpost == null)
        {
            return false;
        }

        _db.Blogposts.Remove(blogpost);
        await _db.SaveChangesAsync();
        return true;
    }

    private static BlogpostDto ToDto(Blogpost blogpost)
    {
        return new BlogpostDto
        {
            Id = blogpost.Id,
            Titel = blogpost.Titel,
            Inhoud = blogpost.Inhoud,
            Publicatiedatum = blogpost.Publicatiedatum
        };
    }
}
