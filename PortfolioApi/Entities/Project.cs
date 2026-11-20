namespace PortfolioApi.Entities;
// ENTITY PROJECT
// Deze class beschrijft de gegevens die EF Core opslaat voor een project.
// De eigenschappen komen overeen met de opgeslagen projectgegevens.
// PortfolioDbContext.cs bevat de DbSet<Project>.
// ProjectService.cs gebruikt deze entity om projecten aan te maken en te wijzigen.
// Id is de primaire sleutel en wordt door SQL Server gegenereerd.
// De overige eigenschappen bevatten de gegevens van het project.
// string.Empty geeft de properties een lege beginwaarde.
public class Project
{
    public int Id { get; set; }
    public string Titel { get; set; } = string.Empty;
    public string Beschrijving { get; set; } = string.Empty;
    public string Categorie { get; set; } = string.Empty;
    public string GitHubUrl { get; set; } = string.Empty;
    public DateTime Datum { get; set; }
}
