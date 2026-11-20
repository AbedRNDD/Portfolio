using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.Dtos.Blogposts;
using PortfolioApi.Entities;

namespace PortfolioApi.Services;
// APPLICATIELAAG VOOR BLOGPOSTS
// Deze service bevat de CRUD methodes voor blogposts.
// De interface staat in Services/IBlogpostService.cs.
// De controller staat in Controllers/BlogpostsController.cs.
public class BlogpostService : IBlogpostService
{
// DEPENDENCY INJECTION
// PortfolioDbContext wordt via de constructor ontvangen.
// De registratie staat in Program.cs.
// De DbContext staat in Data/PortfolioDbContext.cs.
// Via _db.Blogposts kan ik met EF Core de blogposts benaderen.
    private readonly PortfolioDbContext _db;

    public BlogpostService(PortfolioDbContext db)
    {
        _db = db;
    }
// GetAllAsync:
// Haalt alle blogposts op uit SQL Server.
// Gebruikt AsNoTracking omdat de gegevens alleen gelezen worden.
// Zet de Blogpost entities om naar BlogpostDto's.
    public async Task<List<BlogpostDto>> GetAllAsync()
    {
        var blogposts = await _db.Blogposts.AsNoTracking().ToListAsync();
        return blogposts.Select(ToDto).ToList();
    }
// GetByIdAsync:
// Zoekt een blogpost met het opgegeven ID.
// Geeft null terug als de blogpost niet bestaat.
// Zet een gevonden entity om naar BlogpostDto.
    public async Task<BlogpostDto?> GetByIdAsync(int id)
    {
        var blogpost = await _db.Blogposts.AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id);

        return blogpost == null ? null : ToDto(blogpost);
    }
// CreateAsync:
// Ontvangt BlogpostCreateDto en maakt een Blogpost entity.
// Voegt de entity toe via de DbContext en slaat deze op.
// Geeft de opgeslagen gegevens terug als BlogpostDto.
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
// UpdateAsync:
// Zoekt een bestaande blogpost en past de eigenschappen aan.
// SaveChangesAsync slaat de aanpassingen op.
// Geeft true of false terug.
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
// DeleteAsync:
// Zoekt een blogpost en verwijdert deze via de DbContext.
// Geeft true of false terug.
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
// MAPPING: BLOGPOST ENTITY NAAR DTO
// De entity staat in Entities/Blogpost.cs.
// De DTO staat in Dtos/Blogposts/BlogpostDto.cs.
// Deze methode kopieert de eigenschappen van de entity naar de DTO.
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
