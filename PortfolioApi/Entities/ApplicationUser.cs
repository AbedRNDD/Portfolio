using Microsoft.AspNetCore.Identity;

// deze class wordt gebruikt door
// data/portfolioDbContext.cs voor de database
// program.cs voor het instellen voor Identity
// AuthController.cs later voor registratie en login

namespace PortfolioApi.Entities;
public class ApplicationUser : IdentityUser
{

}