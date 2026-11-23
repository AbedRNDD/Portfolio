using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace PortfolioApi.Tests;

// HTTP TESTS - AUTHENTICATIE
//
// Deze class test registratie, inloggen en gebruikersrechten.
//
// TestApiFactory.cs start mijn API met een tijdelijke SQLite database.
// Hierdoor gebruiken de tests niet mijn normale SQL Server database.
//
// Iedere test krijgt een nieuwe TestApiFactory.
// Zo hebben de tests geen invloed op elkaar.

public class AuthTests
{
    // Testgegevens voor het Gast-account.
    private const string Email = "gast@test.local";
    private const string Password = "TestPassword9!";


    // HULPMETHODE - REGISTREREN
    //
    // Deze methode maakt een Gast-account aan.
    // Hierdoor hoef ik dezelfde registratiecode
    // niet in iedere test opnieuw te schrijven.
    //
    // De registratie gebeurt via:
    // AuthController.cs -> Register()

    private static async Task RegisterGast(HttpClient client)
    {
        var response = await client.PostAsJsonAsync(
            "/api/Auth/register",
            new
            {
                email = Email,
                password = Password
            });

        // Een nieuw account moet 201 Created teruggeven.
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }


    // TEST 1 - REGISTRATIE EN GAST-ROL
    //
    // Controleert of een nieuwe gebruiker:
    // 1. Zich kan registreren.
    // 2. Kan inloggen.
    // 3. Automatisch de rol Gast krijgt.
    //
    // Gebruikt:
    // POST /api/Auth/register
    // POST /api/Auth/login
    // GET  /api/Auth/me

    [Fact]
    public async Task Register_NewUser_GetsGastRole()
    {
        // ARRANGE
        // Start de API met een tijdelijke database.
        using var factory = new TestApiFactory();
        using var client = factory.CreateClient();

        // Maak een nieuw Gast-account aan.
        await RegisterGast(client);

        // ACT
        // Log in met het aangemaakte account.
        var login = await client.PostAsJsonAsync(
            "/api/Auth/login",
            new
            {
                email = Email,
                password = Password
            });

        // ASSERT
        // Controleer of het inloggen gelukt is.
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);

        // Vraag de gegevens van de ingelogde gebruiker op.
        var response = await client.GetAsync("/api/Auth/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Lees de JSON-response als tekst.
        var json = await response.Content.ReadAsStringAsync();

        // Zet de JSON om naar een JsonDocument.
        using var document = JsonDocument.Parse(json);

        // Zoek de property "roles" in de JSON-response.
        //
        // AuthController.cs geeft deze property terug:
        //
        // return Ok(new
        // {
        //     email = user.Email,
        //     roles = roles
        // });

        var hasRoles = document.RootElement.TryGetProperty(
            "roles",
            out var rolesElement
        );

        // Controleer of de property bestaat.
        // Bij een fout wordt de ontvangen JSON getoond.
        Assert.True(
            hasRoles,
            $"De response bevat geen 'roles': {json}"
        );

        // Controleer of roles een array is.
        Assert.Equal(JsonValueKind.Array, rolesElement.ValueKind);

        // Zet de rollen om naar een lijst.
        var roles = rolesElement
            .EnumerateArray()
            .Select(role => role.GetString())
            .ToList();

        // Controleer of de nieuwe gebruiker Gast is.
        Assert.Contains("Gast", roles);
    }


    // TEST 2 - DUBBEL E-MAILADRES
    //
    // Een gebruiker mag zich niet twee keer
    // registreren met hetzelfde e-mailadres.
    //
    // Verwachte statuscode: 400 Bad Request.

    [Fact]
    public async Task Register_DuplicateEmail_Returns400()
    {
        // ARRANGE
        using var factory = new TestApiFactory();
        using var client = factory.CreateClient();

        // Registreer het account de eerste keer.
        await RegisterGast(client);

        // ACT
        // Probeer hetzelfde account opnieuw te registreren.
        var response = await client.PostAsJsonAsync(
            "/api/Auth/register",
            new
            {
                email = Email,
                password = Password
            });

        // ASSERT
        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode
        );
    }


    // TEST 3 - VERKEERD WACHTWOORD
    //
    // Een gebruiker met een verkeerd wachtwoord
    // mag niet inloggen.
    //
    // Verwachte statuscode: 401 Unauthorized.

    [Fact]
    public async Task Login_WrongPassword_Returns401()
    {
        // ARRANGE
        using var factory = new TestApiFactory();
        using var client = factory.CreateClient();

        // Maak eerst een account aan.
        await RegisterGast(client);

        // ACT
        // Probeer in te loggen met een verkeerd wachtwoord.
        var response = await client.PostAsJsonAsync(
            "/api/Auth/login",
            new
            {
                email = Email,
                password = "WrongPassword9!"
            });

        // ASSERT
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode
        );
    }


    // TEST 4 EN 5 - GAST HEEFT GEEN SCHRIJFRECHTEN
    //
    // Een Gast mag geen projecten of blogposts toevoegen.
    //
    // Theory voert dezelfde test uit voor twee endpoints.
    //
    // De policy CanManageContent staat in Program.cs.
    // Alleen Editor en Admin hebben schrijfrechten.
    //
    // Verwachte statuscode: 403 Forbidden.

    [Theory]
    [InlineData("/api/Projects")]
    [InlineData("/api/Blogposts")]
    public async Task Gast_CannotCreateContent_Returns403(
        string endpoint)
    {
        // ARRANGE
        using var factory = new TestApiFactory();
        using var client = factory.CreateClient();

        // Registreer een Gast-account.
        await RegisterGast(client);

        // Log in als Gast.
        var login = await client.PostAsJsonAsync(
            "/api/Auth/login",
            new
            {
                email = Email,
                password = Password
            });

        Assert.Equal(HttpStatusCode.OK, login.StatusCode);

        // ACT
        // Stuur een POST-request naar Projects of Blogposts.

        HttpResponseMessage response;

        if (endpoint == "/api/Projects")
        {
            // Probeer een project toe te voegen.
            response = await client.PostAsJsonAsync(
                endpoint,
                new
                {
                    titel = "Testproject",
                    beschrijving = "Project voor xUnit",
                    categorie = "Software",
                    gitHubUrl = "https://github.com/example/test",
                    datum = DateTime.UtcNow
                });
        }
        else
        {
            // Probeer een blogpost toe te voegen.
            response = await client.PostAsJsonAsync(
                endpoint,
                new
                {
                    titel = "Testblogpost",
                    inhoud = "Blogpost voor xUnit",
                    publicatiedatum = DateTime.UtcNow
                });
        }

        // ASSERT
        // Gast mag geen gegevens toevoegen.
        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode
        );
    }
}