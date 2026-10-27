namespace PortfolioApi.Dtos.Blogposts;

public class BlogpostUpdateDto
{
    public string Titel { get; set; } = string.Empty;
    public string Inhoud { get; set; } = string.Empty;
    public DateTime Publicatiedatum { get; set; }
}
