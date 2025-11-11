# Lösning: TypeLoadException - EF Core Versionskonflikt

## Problemet

```
TypeLoadException: Method 'get_LockReleaseBehavior' in type 
'Microsoft.EntityFrameworkCore.SqlServer.Migrations.Internal.SqlServerHistoryRepository' 
from assembly 'Microsoft.EntityFrameworkCore.SqlServer, Version=8.0.13.0, ...' 
does not have an implementation.
```

## Orsak

Felet uppstod på grund av **versionskonflikter** mellan Entity Framework Core NuGet-paket:

```xml
<!-- ? PROBLEM: Olika versioner av EF Core-paket -->
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="9.0.10" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.13" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.13" />
```

När olika EF Core-paket har olika major/minor versioner kan de:
- Ha olika API-signaturer
- Sakna förväntade metoder/properties
- Orsaka runtime TypeLoadException

## Lösningen

**Alla EF Core-paket MÅSTE ha samma version!**

```xml
<!-- ? LÖSNING: Alla paket på version 8.0.13 -->
<PackageReference Include="Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore" Version="8.0.13" />
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.13" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="8.0.13" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.13" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.13" />
```

## Steg för att fixa

### 1. Uppdatera .csproj-filen
Redigera `JobApplicationTrackerV2.csproj` och säkerställ att alla EF Core-paket har samma version.

### 2. Återställ NuGet-paket
```bash
dotnet restore
```

### 3. Rensa gamla builds
```bash
dotnet clean
```

### 4. Bygg om projektet
```bash
dotnet build
```

### 5. Starta om applikationen
Om du kör debugger:
- **Stoppa** debuggern (Shift+F5)
- **Starta om** applikationen (F5)

Hot Reload kan **INTE** hantera paketversionsändringar!

## Varför uppstod detta?

Du har troligtvis:
1. Lagt till SQLite-stöd senare med nyare version
2. Kört `dotnet add package` utan att specificera version
3. Fått den senaste versionen (9.x) istället för den som matchar projektet (8.x)

## Förebyggande åtgärder

### Använd alltid explicit version när du lägger till paket:
```bash
# ? Rätt sätt
dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 8.0.13

# ? Fel sätt (får senaste version)
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

### Använd Directory.Build.props för versionskontroll:
```xml
<Project>
  <PropertyGroup>
 <EFCoreVersion>8.0.13</EFCoreVersion>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Update="Microsoft.EntityFrameworkCore.*" Version="$(EFCoreVersion)" />
  </ItemGroup>
</Project>
```

## Verifiering

Efter fix ska du kunna:
1. ? Bygga projektet utan fel
2. ? Köra migrationer
3. ? Starta applikationen utan TypeLoadException

## Relaterade fel

Detta fel kan också uppstå med:
- `Microsoft.Extensions.*` paket
- `Microsoft.AspNetCore.*` paket
- Andra beroende paket-familjer

**Lösningen är alltid densamma: Använd samma version!**

## När uppgradera till .NET 9?

Om du vill använda EF Core 9.x:
1. Uppgradera **hela projektet** till .NET 9
2. Uppdatera **alla** Microsoft.EntityFrameworkCore.* paket till 9.x
3. Uppdatera **alla** Microsoft.AspNetCore.* paket till 9.x

```xml
<TargetFramework>net9.0</TargetFramework>

<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.10" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="9.0.10" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="9.0.10" />
```

## Lärdomar

1. **Konsekvent versionshantering** är kritiskt i .NET-projekt
2. **Alltid specificera version** när du lägger till NuGet-paket
3. **Hot Reload kan INTE** hantera paketversionsändringar
4. **Starta om** applikationen efter paketändringar

## Referenser

- [EF Core Releases](https://learn.microsoft.com/en-us/ef/core/what-is-new/)
- [NuGet Package Versioning](https://learn.microsoft.com/en-us/nuget/concepts/package-versioning)
- [.NET SDK Compatibility](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core)
