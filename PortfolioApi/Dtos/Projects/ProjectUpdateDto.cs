namespace PortfolioApi.Dtos.Projects;
// INPUT DTO PROJECT AANPASSEN
// Deze DTO bevat de gegevens waarmee een bestaand project wordt aangepast.
// Wordt ontvangen in ProjectsController.cs bij Update().
// ProjectService.UpdateAsync() gebruikt deze gegevens om de entity te wijzigen.
// Het project ID komt uit de URL, bijvoorbeeld PUT /api/Projects/3.
public class ProjectUpdateDto
{
    public string Titel { get; set; } = string.Empty;
    public string Beschrijving { get; set; } = string.Empty;
    public string Categorie { get; set; } = string.Empty;
    public string GitHubUrl { get; set; } = string.Empty;
    public DateTime Datum { get; set; }
}
