namespace PortfolioApi.Entities;
// ENTITY BLOGPOST
// Deze class beschrijft de gegevens die worden opgeslagen voor een blogpost.
// PortfolioDbContext.cs bevat de DbSet<Blogpost>.
// BlogpostService.cs gebruikt deze entity voor databasehandelingen.
// Id is de primaire sleutel.
// Titel en Inhoud bevatten tekst.
// Publicatiedatum bevat de publicatiedatum en tijd.
public class Blogpost
{
    public int Id { get; set; }
    public string Titel { get; set; } = string.Empty;
    public string Inhoud { get; set; } = string.Empty;
    public DateTime Publicatiedatum { get; set; }
}
