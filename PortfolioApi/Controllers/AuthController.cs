using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PortfolioApi.Dtos.Auth;
using PortfolioApi.Entities;

namespace PortfolioApi.Controllers;
//deze controller regelt de registratie en login van gebruikers
//de route wordt ingesteld op api/auth, dus de endpoints zijn api/auth/register en api/auth/login
//gebruikt 
//RegisterDto.cs voor registratie
//LoginDto.cs voor login
//ApplicationUser.cs als gebruikersmodel
// UserManager<ApplicationUser> en SignInManager<ApplicationUser> worden via dependency injection gebruikt in deze controller

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    //Dependency injection
    //userManager beheert gebruikers en rollen
    //signInManager regelt het inloggen van gebruikers en afmelden
    //deze onderdelen worden door identity geregistreerd in Program.cs via addIdentity()

    public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    //Registratie
    //POST api/auth/register
    //iedere gebruiker kan zich registreren
    //editor en admin kunnnen niet via route worden toegekend
    // returntype: Task<ActionResult>
    // task betekent dat de methode asynchroon is en een resultaat teruggeeft
    // actionresult betekent dat de methode een http response teruggeeft
    //iactionsresult betekent dat de methode een http response teruggeeft met een statuscode en eventueel een body

    [HttpPost("register")]
    //AllowAnonymous betekent dat deze methode toegankelijk is voor iedereen, ook voor niet-ingelogde gebruikers
    [AllowAnonymous]
    // we gebruiken async omdat we database operaties uitvoeren die tijd kosten
    public async Task<IActionResult> Register(RegisterDto input)
    {
        var user = new ApplicationUser
        {
            UserName = input.Email,
            Email = input.Email
        };
        // CreateAsync maakt een nieuwe gebruiker aan in de database met het opgegeven wachtwoord
        //await betekent dat we wachten op het resultaat van de asynchrone operatie voordat we verder gaan
        var result = await _userManager.CreateAsync(user, input.Password);
        // Succeeded is true als de gebruiker succesvol is aangemaakt, anders false
        if (!result.Succeeded)
        {
            // Als het aanmaken van de gebruiker niet lukt, geven we een BadRequest terug met de foutmeldingen
            return BadRequest(result.Errors.Select(error=>error.Description));
        }
        // Als de gebruiker succesvol is aangemaakt, voegen we hem toe aan de rol "Gast"
        // AddToRoleAsync voegt de gebruiker toe aan de opgegeven rol
        var roleResult = await _userManager.AddToRoleAsync(user, "Gast");

        //hier !roleResult.Succeeded betekent dat de rol niet succesvol is toegevoegd, dus we verwijderen de gebruiker en geven een foutmelding terug
        if (!roleResult.Succeeded)
        {
            //hier wordt de gebruiker verwijderd als het toevoegen aan de rol niet lukt, zodat er geen half aangemaakte gebruiker in de database blijft staan
            await _userManager.DeleteAsync(user);
            //hier geven we een statuscode 500 terug met een foutmelding, omdat dit een interne serverfout is
            return StatusCode(500, "Registratie kon niet worden afgerond"
            );
        }
        // Als alles goed is gegaan, geven we een statuscode 201 terug met een bericht en de rol van de gebruiker
        return StatusCode(201, new
        {
            message = "Account aangemaakt",
            role = "Gast"
        });
    }

    //Inloggen
    //POST api/auth/login
    //iedere gebruiker kan inloggen met zijn/haar emailadres en wachtwoord
    //UserManager wordt gebruikt om de gebruiker op te zoeken in de database
    //SignInManager wordt gebruikt om de gebruiker te verifiëren en in te loggen
    // bij  succes maakt SignInManager een cookie aan in de browser van de gebruiker, zodat hij/zij ingelogd blijft bij volgende requests
    // returntype: Task<IActionResult>

    [HttpPost("login")]
    [AllowAnonymous]
    // we gebruiken async omdat we database operaties uitvoeren die tijd kosten
    //iactionsresult betekent dat de methode een http response teruggeeft met een statuscode en eventueel een body
    public async Task<IActionResult> Login(LoginDto input)
    {
        // FindByEmailAsync zoekt de gebruiker op in de database op basis van het opgegeven emailadres
        // als de gebruiker niet bestaat, geven we een Unauthorized terug met een foutmelding
        var user = await _userManager.FindByEmailAsync(input.Email);
        // Als de gebruiker niet bestaat, geven we een Unauthorized terug met een foutmelding
        if (user == null)
        {
            return Unauthorized("Ongeldige inloggegevens");
        }
        // PasswordSignInAsync controleert of het opgegeven wachtwoord overeenkomt met het wachtwoord van de gebruiker in de database
        // isPersistent: false betekent dat de cookie niet persistent is, dus de gebruiker wordt uitgelogd als hij/zij de browser sluit
        // lockoutOnFailure: true betekent dat de gebruiker wordt geblokkeerd na een aantal mislukte inlogpogingen, dit is ingesteld in Program.cs bij AddIdentity 
        var result = await _signInManager.PasswordSignInAsync(user, input.Password, isPersistent: false, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            return Unauthorized("Ongeldige inloggegevens");
        }

        return Ok(new
        {
            message = "Inloggen succesvol",
        });
    }
    //Uitloggen
    //POST api/auth/logout
    //alleen ingelogde gebruikers kunnen uitloggen
    //SignInManager wordt gebruikt om de gebruiker uit te loggen en de cookie te verwijderen
    // returntype: Task<IActionResult>
    //iactionsresult betekent dat de methode een http response teruggeeft met een statuscode en eventueel een body

//Authorize betekent dat deze methode alleen toegankelijk is voor ingelogde gebruikers
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        // SignOutAsync logt de gebruiker uit en verwijdert de cookie
        await _signInManager.SignOutAsync();
        return Ok(new
        {
            message = "Uitloggen succesvol"
        });
    }

    //Huidige gebruiker
    //GET api/Auth/me
    // hier kan ik controleren wie is ingelogd en wat zijn/haar rol is
    //UserManager wordt gebruikt om de gebruiker op te zoeken in de database op basis van zijn/haar emailadres
    // returntype: Task<IActionResult>
    //iactionsresult betekent dat de methode een http response teruggeeft met een statuscode en eventueel een body

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        // GetUserAsync haalt de huidige ingelogde gebruiker op uit de database op basis van de cookie
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }
        // GetRolesAsync haalt de rollen van de gebruiker op uit de database
        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new
        {
            email = user.Email,
            role = roles
        });
    }
}