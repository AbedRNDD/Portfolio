using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.Services;

// Dit is het startpunt van mijn API.
// Hier configureer ik de controllers, services, database en Swagger.
// De lokale URL en poort staan in Properties/launchSettings.json.
var builder = WebApplication.CreateBuilder(args);

// Hiermee voeg ik ondersteuning voor controllers toe.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Hiermee configureer ik Swagger.
// Via Swagger kan ik mijn endpoints testen zonder frontend.
builder.Services.AddSwaggerGen();

// Hier is de databaseverbinding uit de configuratie.
// De connectionstring staat in appsettings.Development.json.
var connectionString = builder.Configuration.GetConnectionString("PortfolioDb")
    ?? throw new InvalidOperationException("Connection string 'PortfolioDb' is missing.");

// DEPENDENCY INJECTION DATABASE
// Hier registreer ik PortfolioDbContext en gebruik ik SQL Server.
// De DbContext wordt via de constructor ontvangen in:
// Services/ProjectService.cs
// Services/BlogpostService.cs
builder.Services.AddDbContext<PortfolioDbContext>(options =>
    options.UseSqlServer(connectionString));

// DEPENDENCY INJECTION PROJECTSERVICE/BLOGPOSTSERVICE
// Als een class IProjectService nodig heeft, levert ASP.NET Core ProjectService.
// De interface staat in Services/IProjectService.cs / Services/IBlogpostService.cs
// De uitvoering staat in Services/ProjectService.cs / Services/BlogpostService.cs
// IProjectService / IBlogpostService worden via de constructor ontvangen in:
// Controllers/ProjectsController.cs / Controllers/BlogpostsController.cs
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IBlogpostService, BlogpostService>();

var app = builder.Build();

// Swagger wordt alleen ingeschakeld in de Development omgeving
// Deze omgeving staat ingesteld in Properties/launchSettings.json
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// For this course project we use EnsureCreated instead of migrations.
// It creates the PortfolioDb database and tables the first time the API starts.
// It does NOT delete or reset data on later restarts.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
    db.Database.EnsureCreated();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
