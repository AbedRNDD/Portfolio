using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.Entities;
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

// Asp.Net core identity
// hier voeg ik gebruikersbeheer toe
// ApplicationUser staat in:
// Entities/ApplicationUser.cs
//IdentityRole is een bestaande class van Microsoft.AspNetCore.Identity
//UserManager beheert gebruikers
// SignInManager beheert inloggen
// RoleManager beheert rollen
// deze onderdelen kunnen via dependency injection worden gebruikt in controllers

builder.Services
// AddIdentity voegt de onderdelen van Identity toe
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        // eenzelfde emailadres mag maar 1 keer worden gebruikt
        options.User.RequireUniqueEmail = true;    
    })
    // AddEntityFrameworkStores koppelt Identity aan de database via PortfolioDbContext
    .AddEntityFrameworkStores<PortfolioDbContext>()
    // AddDefaultTokenProviders voegt tokenproviders toe voor wachtwoordreset en emailbevestiging
    .AddDefaultTokenProviders();

// Autorisatie
// dus authenticatie is wie is de gebruiker?
// autorisatie wat mag deze gebruiker doen?
// canmanagecontent geeft schrijfrechten aan gebruikers met de rol editor of admin
// gebruiker met rol gast mag geen content schrijven en aanpassen
// deze policy kan via dependency injection worden gebruikt in controllers

// AddAuthorizationBuilder voegt autorisatie toe
builder.Services.AddAuthorizationBuilder()
//AddPolicy voegt een policy toe met de naam CanManageContent
    .AddPolicy("CanManageContent", policy =>
        policy.RequireRole("Editor", "Admin"));



// applicatie bouwen en starten
var app = builder.Build();

// Swagger wordt alleen ingeschakeld in de Development omgeving
// Deze omgeving staat ingesteld in Properties/launchSettings.json
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//HTTPS, hier kan HTTP worden omgezet naar HTTPS
app.UseHttpsRedirection();


//Authentication
// controleert wie de gebruiker is
// bij cookie authentication wordt de gebruiker ingelogd en kan hij/zij content beheren
// dit moet voor UseAuthorization worden aangeroepen
app.UseAuthentication();

//Authorization
// controleert wat de gebruiker mag doen
app.UseAuthorization();

// Controllers Routes
// hier worden de eindpunten van de controllers geregistreerd
app.MapControllers();

//Rollen aanmaken
//ik maak hier de drie rollen beschikbaar: gast, editor en admin
//RoleManager wordt via dependency injection gebruikt in deze code
// dit gedeelte werkt pas wanneer de Identity databasetabellen via migrations heeft aangemaakt


//
using (var scope = app.Services.CreateScope())
{
    // RoleManager wordt via dependency injection gebruikt in deze code
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = ["Gast", "Editor", "Admin"];
    // Voor elke rol in de array wordt gecontroleerd of deze al bestaat.
    foreach (var role in roles)
    {
        // Als de rol nog niet bestaat, wordt deze aangemaakt.
        if (!await roleManager.RoleExistsAsync(role))
        {
            // De rol wordt aangemaakt met RoleManager.CreateAsync.
            var result = await roleManager.CreateAsync(new IdentityRole(role));
            // Als het aanmaken van de rol niet lukt, wordt een uitzondering gegooid.
            if (!result.Succeeded)
            {
                // Dit kan bijvoorbeeld gebeuren als er een probleem is met de database of de configuratie.
                throw new InvalidOperationException($"could not create role '{role}'.");

            }
        }
    }
}

app.Run();
