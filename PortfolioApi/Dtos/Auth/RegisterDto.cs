using System.ComponentModel.DataAnnotations;

namespace PortfolioApi.Dtos.Auth;

//Input DTO voor registratie van een nieuwe gebruiker
// deze DTO bepaalt welke gegevens de gebruiker moet invullen bij registratie om een account aan te maken
//wordt gebruikt in AuthController.cs bij de Register methode
// de gebruiker mag zelf geen rol kiezen, de rol wordt standaard op "Gast" gezet in AuthController.cs
public class RegisterDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}