using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PortfolioApi.Entities;
using Xunit;

namespace PortfolioApi.Tests;

// HTTP TESTS - CRUD EN AUTORISATIE
//
// Deze tests controleren de rechten van Editor, Admin en Gast.
//
// TestApiFactory start de echte API met een tijdelijke database.
// De tests gebruiken dus niet mijn normale SQL Server database.

public class CrudAuthorizationTests
{
    private const string Password = "TestPassword9!";


    // HULPMETHODE - TESTACCOUNT AANMAKEN
    //
    // Registreert een gebruiker via AuthController.
    // Geeft daarna eventueel de rol Editor of Admin.
    // Vervolgens logt de gebruiker in via de API.
    //
    // UserManager wordt via dependency injection opgehaald.

    private static async Task LoginAsRoleAsync(
        TestApiFactory factory,
        HttpClient client,
        string role)
    {
        var email = $"{Guid.NewGuid():N}@test.local";

        // ARRANGE - Registreer een nieuw account.
        var register = await client.PostAsJsonAsync(
            "/api/Auth/register",
            new
            {
                email,
                password = Password
            });

        Assert.Equal(
            HttpStatusCode.Created,
            register.StatusCode
        );

        // Nieuwe gebruikers krijgen automatisch Gast.
        // Alleen voor tests voegen we Editor of Admin toe.

        if (role != "Gast")
        {
            using var scope = factory.Services.CreateScope();

            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByEmailAsync(email);

            Assert.NotNull(user);

            var result = await userManager.AddToRoleAsync(
                user,
                role
            );

            Assert.True(result.Succeeded);
        }

        // Log in nadat de rol is toegevoegd.
        // De authenticatiecookie bevat dan de juiste rollen.

        var login = await client.PostAsJsonAsync(
            "/api/Auth/login",
            new
            {
                email,
                password = Password
            });

        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
    }


    // HULPMETHODE - TESTGEGEVENS
    //
    // Projects en Blogposts gebruiken verschillende DTO's.
    // Deze methode maakt voor elk endpoint geldige gegevens.

    private static object CreateData(
        string resource,
        string title)
    {
        if (resource == "Projects")
        {
            return new
            {
                titel = title,
                beschrijving = "Beschrijving voor de xUnit-test",
                categorie = "Software",
                gitHubUrl = "https://github.com/example/test",
                datum = DateTime.UtcNow
            };
        }

        return new
        {
            titel = title,
            inhoud = "Inhoud voor de xUnit-test",
            publicatiedatum = DateTime.UtcNow
        };
    }


    // TEST 1 T/M 4 - VOLLEDIGE CRUD
    //
    // Deze test wordt vier keer uitgevoerd:
    // Editor -> Projects
    // Editor -> Blogposts
    // Admin  -> Projects
    // Admin  -> Blogposts
    //
    // Controleert:
    // POST -> gegevens worden aangemaakt.
    // GET  -> gegevens bestaan.
    // PUT  -> gegevens worden aangepast.
    // GET  -> wijziging is opgeslagen.
    // DELETE -> gegevens worden verwijderd.
    // GET  -> gegevens bestaan niet meer.

    [Theory]
    [InlineData("Editor", "Projects")]
    [InlineData("Editor", "Blogposts")]
    [InlineData("Admin", "Projects")]
    [InlineData("Admin", "Blogposts")]
    public async Task AuthorizedUser_CanCreateUpdateDelete(
        string role,
        string resource)
    {
        // ARRANGE
        using var factory = new TestApiFactory();

        using var client = factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

        await LoginAsRoleAsync(factory, client, role);

        var endpoint = $"/api/{resource}";


        // ACT 1 - CREATE
        // Voeg een project of blogpost toe.

        var createResponse = await client.PostAsJsonAsync(
            endpoint,
            CreateData(resource, "Originele titel")
        );

        // ASSERT - POST moet 201 teruggeven.

        Assert.Equal(
            HttpStatusCode.Created,
            createResponse.StatusCode
        );

        // Lees het ID van het nieuwe object.

        var created = await createResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        var id = created.GetProperty("id").GetInt32();

        Assert.True(id > 0);

        Assert.Equal(
            "Originele titel",
            created.GetProperty("titel").GetString()
        );


        // ACT 2 - GET BY ID
        // Controleer of de gegevens zijn opgeslagen.

        var getResponse = await client.GetAsync(
            $"{endpoint}/{id}"
        );

        Assert.Equal(
            HttpStatusCode.OK,
            getResponse.StatusCode
        );

        var saved = await getResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        Assert.Equal(
            "Originele titel",
            saved.GetProperty("titel").GetString()
        );


        // ACT 3 - GET ALL
        // Controleer of het object ook in het overzicht staat.

        var all = await client.GetFromJsonAsync<List<JsonElement>>(
            endpoint
        );

        Assert.NotNull(all);

        Assert.Contains(
            all,
            item => item.GetProperty("id").GetInt32() == id
        );


        // ACT 4 - UPDATE
        // Wijzig de titel van het bestaande object.

        var updateResponse = await client.PutAsJsonAsync(
            $"{endpoint}/{id}",
            CreateData(resource, "Gewijzigde titel")
        );

        Assert.Equal(
            HttpStatusCode.NoContent,
            updateResponse.StatusCode
        );


        // ACT 5 - CONTROLEER UPDATE
        // Haal het object opnieuw op.

        var updatedResponse = await client.GetAsync(
            $"{endpoint}/{id}"
        );

        Assert.Equal(
            HttpStatusCode.OK,
            updatedResponse.StatusCode
        );

        var updated = await updatedResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        Assert.Equal(
            "Gewijzigde titel",
            updated.GetProperty("titel").GetString()
        );


        // ACT 6 - DELETE
        // Verwijder het bestaande object.

        var deleteResponse = await client.DeleteAsync(
            $"{endpoint}/{id}"
        );

        Assert.Equal(
            HttpStatusCode.NoContent,
            deleteResponse.StatusCode
        );


        // ACT 7 - CONTROLEER DELETE
        // Een verwijderd object moet 404 teruggeven.

        var deletedResponse = await client.GetAsync(
            $"{endpoint}/{id}"
        );

        Assert.Equal(
            HttpStatusCode.NotFound,
            deletedResponse.StatusCode
        );
    }


    // TEST 5 EN 6 - GAST MAG NIET WIJZIGEN OF VERWIJDEREN
    //
    // Een Gast is ingelogd, maar heeft geen schrijfrechten.
    // De autorisatiepolicy blokkeert de request.
    //
    // Verwachte statuscode: 403 Forbidden.

    [Theory]
    [InlineData("Projects")]
    [InlineData("Blogposts")]
    public async Task Gast_CannotUpdateOrDelete_Returns403(
        string resource)
    {
        // ARRANGE
        using var factory = new TestApiFactory();

        using var client = factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

        await LoginAsRoleAsync(factory, client, "Gast");

        var endpoint = $"/api/{resource}/12345";


        // ACT - PUT

        var updateResponse = await client.PutAsJsonAsync(
            endpoint,
            CreateData(resource, "Niet toegestaan")
        );

        // ASSERT
        Assert.Equal(
            HttpStatusCode.Forbidden,
            updateResponse.StatusCode
        );


        // ACT - DELETE

        var deleteResponse = await client.DeleteAsync(endpoint);

        // ASSERT
        Assert.Equal(
            HttpStatusCode.Forbidden,
            deleteResponse.StatusCode
        );
    }


    // TEST 7 EN 8 - NIET INGELOGD
    //
    // Zonder authenticatiecookie mogen gebruikers
    // geen gegevens wijzigen of verwijderen.
    //
    // Verwachte statuscode: 401 Unauthorized.

    [Theory]
    [InlineData("Projects")]
    [InlineData("Blogposts")]
    public async Task Anonymous_CannotUpdateOrDelete_Returns401(
        string resource)
    {
        // ARRANGE
        using var factory = new TestApiFactory();

        using var client = factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

        var endpoint = $"/api/{resource}/12345";


        // ACT - PUT

        var updateResponse = await client.PutAsJsonAsync(
            endpoint,
            CreateData(resource, "Niet toegestaan")
        );

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            updateResponse.StatusCode
        );


        // ACT - DELETE

        var deleteResponse = await client.DeleteAsync(endpoint);

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            deleteResponse.StatusCode
        );
    }


    // TEST 9 EN 10 - ONBEKEND ID
    //
    // Editor heeft voldoende rechten.
    // Maar het opgegeven ID bestaat niet.
    //
    // De service moet dan 404 teruggeven.

    [Theory]
    [InlineData("Projects")]
    [InlineData("Blogposts")]
    public async Task Editor_UnknownId_Returns404(
        string resource)
    {
        // ARRANGE
        using var factory = new TestApiFactory();

        using var client = factory.CreateClient();

        await LoginAsRoleAsync(factory, client, "Editor");

        var endpoint = $"/api/{resource}/999999";


        // ACT - PUT

        var updateResponse = await client.PutAsJsonAsync(
            endpoint,
            CreateData(resource, "Onbekend object")
        );

        Assert.Equal(
            HttpStatusCode.NotFound,
            updateResponse.StatusCode
        );


        // ACT - DELETE

        var deleteResponse = await client.DeleteAsync(endpoint);

        Assert.Equal(
            HttpStatusCode.NotFound,
            deleteResponse.StatusCode
        );
    }
}