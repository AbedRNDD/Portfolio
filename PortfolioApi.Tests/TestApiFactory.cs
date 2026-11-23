using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PortfolioApi.Data;

namespace PortfolioApi.Tests;

// Deze factory start mijn API met een tijdelijke testdatabase.
// Hierdoor veranderen de tests niets aan mijn normale SQL Server database.
public class TestApiFactory : WebApplicationFactory<Program>
{
    // De SQLite database blijft bestaan zolang deze verbinding open is.
    private readonly SqliteConnection _connection =
        new("Data Source=:memory:");

    public TestApiFactory()
    {
        _connection.Open();

        // Maak de tabellen aan voor de tijdelijke testdatabase.
        var options = new DbContextOptionsBuilder<PortfolioDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var db = new PortfolioDbContext(options);
        db.Database.EnsureCreated();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureTestServices(services =>
        {
            // Verwijder de normale SQL Server configuratie.
            services.RemoveAll<
                IDbContextOptionsConfiguration<PortfolioDbContext>>();

            services.RemoveAll<
                DbContextOptions<PortfolioDbContext>>();

            // Gebruik SQLite tijdens het testen.
            services.AddDbContext<PortfolioDbContext>(options =>
                options.UseSqlite(_connection));
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            // Sluit de tijdelijke database na het testen.
            _connection.Dispose();
        }
    }
}