using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace PortfolioApi.Tests;

// HTTP TESTS - BLOGPOSTS
//
// Deze tests controleren de openbare GET-routes
// en de beveiliging van de POST-route.

public class BlogpostsTests
{
    // GET /api/Blogposts

    [Fact]
    public async Task GetAll_WithoutLogin_Returns200()
    {
        // ARRANGE
        using var factory = new TestApiFactory();
        using var client = factory.CreateClient();

        // ACT
        var response = await client.GetAsync("/api/Blogposts");

        // ASSERT
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }


    // GET /api/Blogposts/{id}

    [Fact]
    public async Task GetById_UnknownId_Returns404()
    {
        // ARRANGE
        using var factory = new TestApiFactory();
        using var client = factory.CreateClient();

        // ACT
        var response = await client.GetAsync("/api/Blogposts/999999");

        // ASSERT
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }


    // POST /api/Blogposts
    //
    // Zonder login mag ik geen blogpost toevoegen.

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

        var blogpost = new
        {
            titel = "Testblogpost",
            inhoud = "Een blogpost voor mijn xUnit-test.",
            publicatiedatum = DateTime.UtcNow
        };

        // ACT
        var response = await client.PostAsJsonAsync(
            "/api/Blogposts",
            blogpost
        );

        // ASSERT
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode
        );
    }
}