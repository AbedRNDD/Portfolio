namespace PortfolioApi.Dtos.Blogposts;
// INPUT DTO BLOGPOST AANPASSEN
// Deze DTO bevat de nieuwe gegevens van een bestaande blogpost.
// Wordt ontvangen in BlogpostsController.cs bij Update().
// BlogpostService.UpdateAsync() past de bestaande entity hiermee aan.
// Het Id wordt via de URL doorgegeven.
public class BlogpostUpdateDto
{
    public string Titel { get; set; } = string.Empty;
    public string Inhoud { get; set; } = string.Empty;
    public DateTime Publicatiedatum { get; set; }
}
