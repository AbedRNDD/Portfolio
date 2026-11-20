namespace PortfolioApi.Dtos.Projects;
// INPUT DTO PROJECT TOEVOEGEN
// Deze DTO beschrijft welke gegevens een POST-request moet bevatten.
// Wordt ontvangen in ProjectsController.cs bij Create().
// ProjectService.CreateAsync() maakt hiervan een Project entity.
// Deze DTO heeft geen Id omdat SQL Server dat automatisch genereert.
public class ProjectCreateDto
{
    public string Titel { get; set; } = string.Empty;
    public string Beschrijving { get; set; } = string.Empty;
    public string Categorie { get; set; } = string.Empty;
    public string GitHubUrl { get; set; } = string.Empty;
    public DateTime Datum { get; set; }
}
