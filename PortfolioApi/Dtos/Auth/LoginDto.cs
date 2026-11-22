using System.ComponentModel.DataAnnotations;

namespace PortfolioApi.Dtos.Auth;

// DTO voor registratie van een nieuwe gebruiker
//dete dto bevat het emailadres en wachtwoord van de gebruiker
//wordt gebruikt in AuthController.cs bij de Login methode
//SignInManager<ApplicationUser> wordt gebruikt om de gebruiker te verifiëren
public class LoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}