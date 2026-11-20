using Microsoft.EntityFrameworkCore;
using PortfolioApi.Entities;

namespace PortfolioApi.Data;
// DATA ACCESS LAYER
// Deze class erft van EF Core DbContext.
// Hiermee kunnen mijn services gegevens ophalen en opslaan in SQL Server.
// De registratie staat in Program.cs.
// De databaseverbinding staat in appsettings.Development.json.
// ProjectService.cs en BlogpostService.cs ontvangen deze DbContext
// via hun constructors door dependency injection.
public class PortfolioDbContext : DbContext
{
// De constructor ontvangt DbContextOptions vanuit dependency injection.
// Program.cs configureert deze opties met UseSqlServer.
// base(options) geeft deze configuratie door aan de EF Core DbContext.
    public PortfolioDbContext(DbContextOptions<PortfolioDbContext> options)
        : base(options)
    {
    }
// Deze DbSet geeft toegang tot de Project entities / blogpost entities.
// De Project entity staat in Entities/Project.cs / Blogpost entity staat in Entities/Blogpost.cs.
// EF Core koppelt deze aan de Projects tabel / Blogposts tabel in SQL Server.
// ProjectService.cs / BlogpostService.cs gebruiken deze DbSet via _db.Projects / _db.Blogposts.
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Blogpost> Blogposts => Set<Blogpost>();
}
