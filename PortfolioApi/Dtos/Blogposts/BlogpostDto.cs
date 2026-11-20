namespace PortfolioApi.Dtos.Blogposts;
// OUTPUT DTO BLOGPOSTGEGEVENS TERUGGEVEN
// Deze DTO bevat de gegevens die de API terugstuurt.
// BlogpostService.cs zet de Blogpost entity om naar deze DTO met ToDto().
// BlogpostsController.cs gebruikt deze DTO in GET- en POST-responses.
// Het Id staat hier wel in, zodat de client de blogpost kan herkennen.
public class BlogpostDto
{
    public int Id { get; set; }
    public string Titel { get; set; } = string.Empty;
    public string Inhoud { get; set; } = string.Empty;
    public DateTime Publicatiedatum { get; set; }
}
