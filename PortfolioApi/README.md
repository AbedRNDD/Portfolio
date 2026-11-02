# PortfolioApi - WPFW Opdracht 3

Voor opdracht 3 heb ik een backend gemaakt voor mijn portfolio met ASP.NET Core Web API (.NET 10).

Met deze API kan ik projecten en blogposts toevoegen, bekijken, aanpassen en verwijderen. De gegevens worden opgeslagen in SQL Server met EF Core. Mijn bestaande website staat nog gewoon in de hoofdmap van Portfolio. Die is nog niet gekoppeld aan deze API. Ik test de backend daarom via Swagger.

## Wat heb je nodig?

.NET 10 SDK (ik heb versie 10.0.403 gebruikt)
SQL Server Express
Windows-authenticatie voor SQL Server
SSMS om de database te bekijken (handig, maar niet nodig om de API te starten)

## Hoe heb ik het ingedeeld?

Ik heb gekozen voor **controllers** en de code verdeeld in mappen. Zo heeft ieder onderdeel zijn eigen taak en blijft het overzichtelijk.


PortfolioApi/
Controllers/   Ontvangen de aanvragen en geven een antwoord terug
Services/      Voeren de acties uit, zoals toevoegen en verwijderen
Data/          Bevat PortfolioDbContext voor de database
Entities/      De gegevens die in de database worden opgeslagen
Dtos/          De gegevens die de API ontvangt en teruggeeft
Properties/    Instellingen voor het starten
Program.cs     Stelt de API, services en databaseverbinding in
PortfolioApi.csproj

Bijvoorbeeld als ik alle projecten opvraag:

Swagger -> ProjectsController -> ProjectService -> PortfolioDbContext -> SQL Server

Daarna stuurt de API de projecten terug als JSON. De controller praat dus niet zelf rechtstreeks met de database.

## SQL Server en de database

Ik gebruik een database met de naam PortfolioDb op SQL Server Express. De verbinding staat in `appsettings.Development.json`.

Het belangrijkste deel daarvan is:

```json
{
  "ConnectionStrings": {
    "PortfolioDb": "Server=.\\SQLEXPRESS;Database=PortfolioDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Dit is een voorbeeld van het verbindingsgedeelte. Andere instellingen in het bestand kunnen blijven staan. Als SQL Server op een andere servernaam draait, moet je `Server` aanpassen.

Ik heb **EnsureCreated()** gebruikt in `Program.cs`. Als `PortfolioDb` nog niet bestaat, maakt EF Core de database en de tabellen `Projects` en `Blogposts` aan. De gegevens blijven bestaan als ik de API afsluit en weer opstart.

`EnsureCreated()` past bestaande tabellen niet automatisch aan als ik later velden verander.

In SSMS vind ik de tabellen onder:

`Databases → PortfolioDb → Tables`

## Hoe start je de API?

Open de map Portfolio in VS Code. Voer in de terminal uit:

```powershell
cd PortfolioApi
dotnet restore
dotnet run
```

Als de API is gestart, kun je Swagger openen op:

`http://localhost:5168/swagger`

Dit adres hoort bij mijn lokale startinstellingen. Als de terminal een andere poort laat zien, gebruik je die poort.

Met Ctrl + C stop je de API weer.

## Wat kan de API doen?

Voor projecten en blogposts heb ik dezelfde vijf acties gemaakt:

| Actie | Projecten | Blogposts |

| Alles bekijken | `GET /api/Projects` | `GET /api/Blogposts` |
| Een item bekijken | `GET /api/Projects/{id}` | `GET /api/Blogposts/{id}` |
| Toevoegen | `POST /api/Projects` | `POST /api/Blogposts` |
| Aanpassen | `PUT /api/Projects/{id}` | `PUT /api/Blogposts/{id}` |
| Verwijderen | `DELETE /api/Projects/{id}` | `DELETE /api/Blogposts/{id}` |

Bij toevoegen krijg ik 201 Created. Bij ophalen 200 OK. Bij aanpassen en verwijderen 204 No Content. Als een ID niet bestaat, krijg ik 404 Not Found.

## DTO's en testen

Ik gebruik aparte DTO's voor het toevoegen, aanpassen en teruggeven van projecten en blogposts. Bij het toevoegen stuur ik zelf geen ID mee. Die wordt door SQL Server gemaakt.

Ik heb alle endpoints getest in Swagger. Eerst heb ik een project en blogpost toegevoegd, daarna opgehaald en aangepast. Daarna heb ik de API opnieuw gestart en gecontroleerd of de aangepaste gegevens er nog waren. Als laatste heb ik de items verwijderd en met GET gecontroleerd dat ik 404 terugkreeg.
