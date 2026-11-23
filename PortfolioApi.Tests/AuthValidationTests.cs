using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace PortfolioApi.Tests;

// HTTP TESTS - VALIDATIE EN BEVEILIGING
//
// Deze tests controleren ongeldige invoer,
// gebruikersrollen en authenticatiecookies.
//
// Elke test gebruikt een eigen tijdelijke database.

public class AuthValidationTests
{
    private const string Email = "gast@test.local";
    private const string Password = "TestPassword9!";


    // HULPMETHODE - REGISTRATIE
    //
    // Maakt een Gast-account aan via AuthController.

    private static async Task RegisterGast(HttpClient client)
    {
        var response = await client.PostAsJsonAsync(
            "/api/Auth/register",
            new
            {
                email = Email,
                password = Password
            });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }


    // TEST 1 T/M 3 - ONGELDIGE REGISTRATIE
    //
    // Controleert:
    // - Ongeldig e-mailadres.
    // - Leeg e-mailadres.
    // - Leeg wachtwoord.
    //
    // RegisterDto gebruikt Required en EmailAddress.
    // Daarom verwacht ik 400 Bad Request.

    [Theory]
    [InlineData("geen-email", "TestPassword9!")]
    [InlineData("", "TestPassword9!")]
    [InlineData("gast@test.local", "")]
    public async Task Register_InvalidInput_Returns400(
        string email,
        string password)
    {
        // ARRANGE
        using var factory = new TestApiFactory();
        using var client = factory.CreateClient();

        // ACT
        var response = await client.PostAsJsonAsync(
            "/api/Auth/register",
            new
            {
                email,
                password
            });

        // ASSERT
        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode
        );
    }


    // TEST 4 - ONBEKENDE GEBRUIKER
    //
    // Inloggen met een onbekend e-mailadres
    // moet 401 Unauthorized teruggeven.

    [Fact]
    public async Task Login_UnknownUser_Returns401()
    {
        // ARRANGE
        using var factory = new TestApiFactory();
        using var client = factory.CreateClient();

        // ACT
        var response = await client.PostAsJsonAsync(
            "/api/Auth/login",
            new
            {
                email = "onbekend@test.local",
                password = Password
            });

        // ASSERT
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode
        );
    }


    // TEST 5 - GEBRUIKER MAG GEEN ADMIN KIEZEN
    //
    // Iemand probeert zich te registreren als Admin.
    // De API mag deze rol niet overnemen.
    //
    // Nieuwe accounts moeten altijd Gast worden.

    [Fact]
    public async Task Register_CannotChooseAdminRole()
    {
        // ARRANGE
        using var factory = new TestApiFactory();
        using var client = factory.CreateClient();

        // ACT
        // Probeer tijdens registratie Admin mee te sturen.

        var register = await client.PostAsJsonAsync(
            "/api/Auth/register",
            new
            {
                email = Email,
                password = Password,
                role = "Admin"
            });

        // Registratie zelf mag slagen.
        Assert.Equal(
            HttpStatusCode.Created,
            register.StatusCode
        );

        // Log in met het nieuwe account.
        var login = await client.PostAsJsonAsync(
            "/api/Auth/login",
            new
            {
                email = Email,
                password = Password
            });

        Assert.Equal(HttpStatusCode.OK, login.StatusCode);

        // Haal de daadwerkelijke gebruikersrollen op.
        var response = await client.GetAsync("/api/Auth/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var user = await response.Content
            .ReadFromJsonAsync<JsonElement>();

        var roles = user.GetProperty("roles")
            .EnumerateArray()
            .Select(role => role.GetString())
            .ToList();

        // ASSERT
        // De gebruiker moet Gast zijn, niet Admin.
        Assert.Contains("Gast", roles);
        Assert.DoesNotContain("Admin", roles);
    }


    // TEST 6 - COOKIE EN UITLOGGEN
    //
    // Controleert dat:
    // 1. Login een authenticatiecookie teruggeeft.
    // 2. De cookie toegang geeft tot /me.
    // 3. Uitloggen de sessie beëindigt.
    // 4. /me daarna 401 teruggeeft.

    [Fact]
    public async Task Login_CookieAndLogout_Works()
    {
        // ARRANGE
        using var factory = new TestApiFactory();

        using var client = factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                HandleCookies = true
            });

        await RegisterGast(client);

        // ACT 1 - INLOGGEN
        var login = await client.PostAsJsonAsync(
            "/api/Auth/login",
            new
            {
                email = Email,
                password = Password
            });

        Assert.Equal(HttpStatusCode.OK, login.StatusCode);

        // ASSERT - Controleer of de server een cookie verstuurt.
        var hasCookie = login.Headers.TryGetValues(
            "Set-Cookie",
            out var cookies
        );

        Assert.True(hasCookie);
        Assert.NotEmpty(cookies!);

        // ACT 2 - GEBRUIKER OPHALEN
        // De testclient verstuurt de cookie automatisch.

        var me = await client.GetAsync("/api/Auth/me");

        Assert.Equal(HttpStatusCode.OK, me.StatusCode);

        // ACT 3 - UITLOGGEN
        var logout = await client.PostAsync(
            "/api/Auth/logout",
            null
        );

        Assert.Equal(HttpStatusCode.OK, logout.StatusCode);

        // ACT 4 - OPNIEUW GEBRUIKER OPHALEN
        // Na uitloggen mag /me niet meer toegankelijk zijn.

        var afterLogout = await client.GetAsync("/api/Auth/me");

        // ASSERT
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            afterLogout.StatusCode
        );
    }
}