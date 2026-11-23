using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace PortfolioApi.Tests;

// HTTP TESTS - PROJECTS
//
// Deze tests sturen echte HTTP-requests naar mijn API.
// TestApiFactory start de API met een tijdelijke database.
//
// Arrange = test voorbereiden.
// Act = request uitvoeren.
// Assert = controleren of het resultaat klopt.

public class ProjectsTests
{
    // GET /api/Projects
    //
    // Iedereen mag projecten ophalen.
    // Daarom verwacht ik 200 OK.

    [Fact]
    public async Task GetAll_WithoutLogin_Returns200()
    {
        // ARRANGE
        using var factory = new TestApiFactory();
        using var client = factory.CreateClient();

        // ACT
        var response = await client.GetAsync("/api/Projects");

        // ASSERT
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }


    // GET /api/Projects/{id}
    //
    // Een niet-bestaand project geeft 404 Not Found.

    [Fact]
    public async Task GetById_UnknownId_Returns404()
    {
        // ARRANGE
        using var factory = new TestApiFactory();
        using var client = factory.CreateClient();

        // ACT
        var response = await client.GetAsync("/api/Projects/999999");

        // ASSERT
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }


    // POST /api/Projects
    //
    // Zonder inloggen mag ik geen project toevoegen.
    // Daarom verwacht ik 401 Unauthorized.

    [Fact]
    public async Task Create_WithoutLogin_Returns401()
    {
        // ARRANGE
        using var factory = new TestApiFactory();

        using var client = factory.CreateClient(
            new Microsoft.AspNetCore.Mvc.Testing
                .WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

        var project = new
        {
            titel = "Testproject",
            beschrijving = "Project voor xUnit",
            categorie = "Software",
            gitHubUrl = "https://github.com/example/test",
            datum = DateTime.UtcNow
        };

        // ACT
        var response = await client.PostAsJsonAsync(
            "/api/Projects",
            project
        );

        // ASSERT
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode
        );
    }
}