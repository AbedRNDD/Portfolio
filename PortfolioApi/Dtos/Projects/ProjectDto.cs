namespace PortfolioApi.Dtos.Projects;
// OUTPUT DTO PROJECTGEGEVENS TERUGGEVEN
// Deze DTO bepaalt welke projectgegevens de API terugstuurt.
// ProjectService.cs zet Project entities om naar deze DTO via ToDto().
// ProjectsController.cs gebruikt deze DTO in de GET- en POST-responses.
// Deze DTO bevat wel het Id, zodat de client het project kan herkennen.
public class ProjectDto
{
    public int Id { get; set; }
    public string Titel { get; set; } = string.Empty;
    public string Beschrijving { get; set; } = string.Empty;
    public string Categorie { get; set; } = string.Empty;
    public string GitHubUrl { get; set; } = string.Empty;
    public DateTime Datum { get; set; }
}
