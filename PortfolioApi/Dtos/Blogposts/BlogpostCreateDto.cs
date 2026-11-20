namespace PortfolioApi.Dtos.Blogposts;
// INPUT DTO BLOGPOST TOEVOEGEN
// Deze DTO bevat de gegevens voor een nieuwe blogpost.
// Wordt ontvangen in BlogpostsController.cs bij Create().
// BlogpostService.CreateAsync() maakt hiervan een Blogpost entity.
// Het Id ontbreekt omdat SQL Server het automatisch aanmaakt.
public class BlogpostCreateDto
{
    public string Titel { get; set; } = string.Empty;
    public string Inhoud { get; set; } = string.Empty;
    public DateTime Publicatiedatum { get; set; }
}
