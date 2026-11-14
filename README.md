# RecipeHub

Webová aplikace pro prohlížení a správu receptů vytvořená v ASP.NET Core MVC.
Projekt vznikl jako školní práce ve dvoučlenném týmu. Po jejím dokončení jsem projekt dále upravil a dotáhl do současné podoby. odkaz na puvodni repo - https://github.com/RadoslavLabon/RecipeHub

## Funkce

- Prohlížení receptů a filtrování podle kategorií
- Registrace, přihlášení a úprava profilu
- Přidávání, úprava a mazání vlastních receptů
- Hodnocení receptů přihlášenými uživateli

## Technologie

- .NET 9 a ASP.NET Core MVC
- Entity Framework Core a SQL Server
- ASP.NET Core Identity
- Razor, Bootstrap a JavaScript

## Spuštění

Je potřeba .NET 9 SDK a SQL Server. Výchozí nastavení používá SQL Server LocalDB na Windows. Připojení lze změnit v `RecipeHub/appsettings.json` nebo pomocí user secrets.

V kořenovém adresáři projektu spusť:

```powershell
dotnet tool restore
dotnet restore RecipeHub.sln
dotnet ef database update --project RecipeHub.Infrastructure --startup-project RecipeHub
dotnet run --project RecipeHub --launch-profile http
```

Aplikace poběží na [http://localhost:5018](http://localhost:5018). Při prvním spuštění se do prázdné databáze přidají ukázkové recepty a kategorie. Pro přidávání a hodnocení receptů je potřeba si založit účet.

Po změně databázového modelu nebo aktualizaci projektu je potřeba znovu spustit příkaz `dotnet ef database update`. Pokud starší databáze obsahuje více hodnocení stejného receptu od jednoho uživatele, je před použitím nové migrace potřeba duplicity odstranit.

## Testy

```powershell
dotnet test RecipeHub.sln
```

Testy používají SQLite databázi v paměti, takže pro ně není potřeba SQL Server.


