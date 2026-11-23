Voor opdracht 3 heb ik een backend gemaakt voor mijn portfolio met ASP.NET Core Web API (.NET 10).

Met deze API kan ik projecten en blogposts toevoegen, bekijken, aanpassen en verwijderen. De gegevens worden opgeslagen in SQL Server met EF Core. Mijn bestaande website staat nog gewoon in de hoofdmap van Portfolio. Die is nog niet gekoppeld aan deze API. Ik test de backend daarom via Swagger.

Voor opdracht 4 heb ik de API beveiligd met ASP.NET Core Identity. Gebruikers kunnen zich registreren en inloggen. Ik heb ook rollen toegevoegd en de API getest met xUnit.
Wat heb je nodig?
- .NET 10 SDK (ik heb versie 10.0.403 gebruikt)
- SQL Server Express
- Windows-authenticatie voor SQL Server
- SSMS om de database te bekijken en testrollen toe te kennen (handig, maar niet nodig om de API te starten)
- De EF Core CLI-tool dotnet-ef voor de migraties
Hoe heb ik het ingedeeld?
Ik heb gekozen voor controllers en de code verdeeld in mappen. Zo heeft ieder onderdeel zijn eigen taak en blijft het overzichtelijk.
Portfolio/
├── PortfolioApi/
│   ├── Controllers/   Ontvangen de aanvragen en geven een antwoord terug
│   ├── Services/      Voeren de acties uit, zoals toevoegen en verwijderen
│   ├── Data/          Bevat PortfolioDbContext voor de database
│   ├── Entities/      De gegevens die in de database worden opgeslagen
│   ├── Dtos/          De gegevens die de API ontvangt en teruggeeft
│   ├── Migrations/    Bevat de databasewijzigingen van EF Core
│   ├── Properties/    Instellingen voor het starten
│   ├── Program.cs     Stelt de API, services, Identity en database in
│   └── PortfolioApi.csproj
├── PortfolioApi.Tests/  Bevat mijn xUnit-tests
└── Portfolio.slnx
Bijvoorbeeld als ik alle projecten opvraag:

Swagger → ProjectsController → ProjectService → PortfolioDbContext → SQL Server

Daarna stuurt de API de projecten terug als JSON. De controller praat dus niet zelf rechtstreeks met de database.

Voor registratie en inloggen gebruikt AuthController de onderdelen UserManager en SignInManager van Identity.
SQL Server en de database
Bij opdracht 3 gebruikte ik een database met de naam PortfolioDb op SQL Server Express. Daarvoor gebruikte ik EnsureCreated() in Program.cs.

Voor opdracht 4 heb ik een aparte database gemaakt: PortfolioDbOpdracht4Migrations. Hiervoor gebruik ik EF Core Migrations in plaats van EnsureCreated(). Mijn oude database blijft bestaan.

De verbinding staat in PortfolioApi/appsettings.Development.json. Het belangrijkste deel daarvan is:
{
  "ConnectionStrings": {
    "PortfolioDb": "Server=.\\SQLEXPRESS;Database=PortfolioDbOpdracht4Migrations;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
Dit is een voorbeeld van het verbindingsgedeelte. Andere instellingen in het bestand kunnen blijven staan. Als SQL Server op een andere servernaam draait, moet je Server aanpassen.

Om de bestaande migratie uit te voeren, open ik de hoofdmap Portfolio in de terminal:
dotnet ef database update --project .\PortfolioApi\PortfolioApi.csproj --connection "Server=.\SQLEXPRESS;Database=PortfolioDbOpdracht4Migrations;Trusted_Connection=True;TrustServerCertificate=True;"
Als dotnet ef niet wordt herkend, kan ik de tool eerst installeren:
dotnet tool install --global dotnet-ef --version 10.0.14
De migratie maakt de tabellen voor projecten en blogposts aan, maar ook de Identity-tabellen, zoals AspNetUsers, AspNetRoles en AspNetUserRoles. In __EFMigrationsHistory houdt EF Core bij welke migraties zijn uitgevoerd.

In SSMS vind ik de tabellen onder:

Databases → PortfolioDbOpdracht4Migrations → Tables
Hoe start je de API?
Open de map Portfolio in VS Code. Voer in de terminal uit:
cd PortfolioApi
dotnet restore
dotnet run
Als de API is gestart, kun je Swagger openen op:

http://localhost:5168/swagger

Dit adres hoort bij mijn lokale startinstellingen. Als de terminal een andere poort laat zien, gebruik je die poort.

Met Ctrl + C stop je de API weer.
Wat kan de API doen?
Voor projecten en blogposts heb ik dezelfde vijf acties gemaakt:
Actie	Projecten	Blogposts
Alles bekijken	GET /api/Projects	GET /api/Blogposts
Eén item bekijken	GET /api/Projects/{id}	GET /api/Blogposts/{id}
Toevoegen	POST /api/Projects	POST /api/Blogposts
Aanpassen	PUT /api/Projects/{id}	PUT /api/Blogposts/{id}
Verwijderen	DELETE /api/Projects/{id}	DELETE /api/Blogposts/{id}


Bij toevoegen krijg ik 201 Created. Bij ophalen 200 OK. Bij aanpassen en verwijderen 204 No Content. Als een ID niet bestaat, krijg ik 404 Not Found.
Registratie en inloggen
Voor opdracht 4 heb ik AuthController toegevoegd. Deze controller heeft vier routes:
Actie	Route	Resultaat bij succes
Registreren	POST /api/Auth/register	201 Created
Inloggen	POST /api/Auth/login	200 OK
Uitloggen	POST /api/Auth/logout	200 OK
Ingelogde gebruiker bekijken	GET /api/Auth/me	200 OK


Bij registratie geef ik een e-mailadres en wachtwoord op. Identity controleert de gegevens en slaat het wachtwoord op als een hash. Een nieuw account krijgt automatisch de rol Gast.

Bij het inloggen controleert SignInManager het wachtwoord. Als dat klopt, krijgt de gebruiker een authenticatiecookie. Via /api/Auth/me kan ik controleren met welk account ik ben ingelogd en welke rollen dat account heeft.
Rollen en beveiliging
Ik gebruik drie rollen:
- Gast: kan projecten en blogposts bekijken.
- Editor: kan projecten en blogposts bekijken, toevoegen, aanpassen en verwijderen.
- Admin: heeft voor deze API dezelfde schrijfrechten als Editor.
In Program.cs heb ik de policy CanManageContent gemaakt. Die staat alleen de rollen Editor en Admin toe. In ProjectsController en BlogpostsController gebruik ik deze policy bij POST, PUT en DELETE. GET blijft openbaar.

Als iemand niet is ingelogd en iets probeert toe te voegen of aan te passen, geeft de API 401 Unauthorized terug. Een ingelogde Gast krijgt bij dezelfde actie 403 Forbidden.

Nieuwe gebruikers kunnen bij registratie niet zelf Editor of Admin kiezen. Voor mijn lokale tests heb ik twee accounts eerst als Gast geregistreerd en daarna handmatig een extra rol gegeven in SSMS.

Dit kan ik uitvoeren in een nieuw queryvenster van SSMS, nadat editor@example.com en admin@example.com via Swagger zijn geregistreerd:
USE PortfolioDbOpdracht4Migrations;

-- Geef het testaccount de rol Editor.
INSERT INTO AspNetUserRoles (UserId, RoleId)
SELECT u.Id, r.Id
FROM AspNetUsers u
CROSS JOIN AspNetRoles r
WHERE u.NormalizedEmail = 'EDITOR@EXAMPLE.COM'
  AND r.NormalizedName = 'EDITOR'
  AND NOT EXISTS (
      SELECT 1 FROM AspNetUserRoles ur
      WHERE ur.UserId = u.Id AND ur.RoleId = r.Id
  );

-- Geef het andere testaccount de rol Admin.
INSERT INTO AspNetUserRoles (UserId, RoleId)
SELECT u.Id, r.Id
FROM AspNetUsers u
CROSS JOIN AspNetRoles r
WHERE u.NormalizedEmail = 'ADMIN@EXAMPLE.COM'
  AND r.NormalizedName = 'ADMIN'
  AND NOT EXISTS (
      SELECT 1 FROM AspNetUserRoles ur
      WHERE ur.UserId = u.Id AND ur.RoleId = r.Id
  );
Daarna log ik uit en opnieuw in, zodat de cookie de nieuwe rollen bevat. Via GET /api/Auth/me controleer ik of de rol goed is toegevoegd. Ik gebruik deze SQL alleen voor mijn lokale testaccounts.
DTO's en testen
Ik gebruik aparte DTO's voor het toevoegen, aanpassen en teruggeven van projecten en blogposts. Bij het toevoegen stuur ik zelf geen ID mee. Die wordt door SQL Server gemaakt.

Bij opdracht 3 heb ik alle endpoints getest in Swagger. Eerst heb ik een project en blogpost toegevoegd, daarna opgehaald en aangepast. Daarna heb ik de API opnieuw gestart en gecontroleerd of de aangepaste gegevens er nog waren. Als laatste heb ik de items verwijderd en met GET gecontroleerd dat ik 404 terugkreeg.

Voor opdracht 4 heb ik naast de Swagger-tests ook een apart xUnit-testproject gemaakt: PortfolioApi.Tests. Deze tests starten de API met WebApplicationFactory en gebruiken een tijdelijke SQLite-database. Daardoor veranderen ze niets aan mijn normale SQL Server-database.

Ik heb de tests verdeeld over de volgende bestanden:
- ProjectsTests.cs en BlogpostsTests.cs controleren de openbare GET-routes, onbekende ID's en beveiligde POST-routes.
- AuthTests.cs controleert registratie, inloggen en de rechten van Gast.
- CrudAuthorizationTests.cs controleert de rechten van Editor en Admin en of toevoegen, aanpassen en verwijderen echt werken.
- AuthValidationTests.cs controleert ongeldige invoer, rollen en het gebruik van de authenticatiecookie.
Mijn tests gebruiken Arrange, Act, Assert. Ik controleer niet alleen de statuscodes, maar bij de CRUD-tests ook of de gegevens daadwerkelijk zijn toegevoegd, aangepast of verwijderd.

De tests voer ik vanuit de hoofdmap Portfolio uit met:
dotnet test .\PortfolioApi.Tests\PortfolioApi.Tests.csproj
Mijn testset bestaat uit 27 tests. Hiermee controleer ik onder andere de statuscodes 200, 201, 204, 400, 401, 403 en 404.