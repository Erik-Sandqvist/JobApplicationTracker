# JobApplicationTrackerV2 - Tester

Detta projekt innehåller omfattande enhetstester för JobApplicationTrackerV2-applikationen.

## Testöversikt

Projektet innehåller **59 tester** fördelade över tre testkategorier:

### 1. Modeltester (`JobApplicationModelTests`) - 13 tester
Testar `JobApplication`-modellen och dess valideringsregler:
- ? Standardvärden vid skapande
- ? Validering av obligatoriska fält (Företag, Jobbtitel, UserId)
- ? Längdvalidering (max 200 tecken för företag/jobbtitel, 1000 för anteckningar)
- ? URL-validering
- ? Alla status-värden fungerar korrekt
- ? Alla properties kan sättas och läsas

### 2. Repository/Databastester (`JobApplicationRepositoryTests`) - 13 tester
Testar databasoperationer med Entity Framework Core InMemory:
- ? Skapa jobbansökningar
- ? Hämta ansökningar per ID
- ? Uppdatera ansökningar
- ? Ta bort ansökningar
- ? Filtrera per användare (UserId)
- ? Sortera efter datum
- ? Filtrera per status
- ? Räkna ansökningar per status
- ? Söka efter företagsnamn
- ? Multi-user isolering
- ? Lagra med alla/minimala fält

### 3. Enum-tester (`ApplicationStatusTests`) - 33 tester
Testar `ApplicationStatus` enum:
- ? Alla status-värden finns
- ? Korrekta integer-värden
- ? Konvertering från/till integer
- ? ToString()-funktionalitet
- ? Parsing från sträng
- ? Default-värde (VantarPaSvar)

## Köra testerna

### Kör alla tester
```powershell
dotnet test Tests\JobApplicationTrackerV2.Tests.csproj
```

### Kör med detaljerad output
```powershell
dotnet test Tests\JobApplicationTrackerV2.Tests.csproj --verbosity detailed
```

### Kör specifik testklass
```powershell
dotnet test Tests\JobApplicationTrackerV2.Tests.csproj --filter "FullyQualifiedName~JobApplicationModelTests"
```

### Kör specifikt test
```powershell
dotnet test Tests\JobApplicationTrackerV2.Tests.csproj --filter "FullyQualifiedName~CanAddJobApplicationToDatabase"
```

## Teknologier

- **xUnit** 2.9.2 - Testramverk
- **FluentAssertions** 8.8.0 - Assertions-bibliotek för mer läsbar testkod
- **Microsoft.EntityFrameworkCore.InMemory** 9.0.10 - InMemory-databas för tester
- **Moq** 4.20.72 - Mocking-bibliotek (för framtida tester)

## Teststruktur

```
Tests/
??? JobApplicationModelTests.cs      # Modell- och valideringstester
??? JobApplicationRepositoryTests.cs # Databas- och CRUD-tester
??? ApplicationStatusTests.cs      # Enum-tester
??? Usings.cs              # Globala using-direktiv
```

## Best Practices

- **AAA-mönstret**: Alla tester följer Arrange-Act-Assert-strukturen
- **Isolerade tester**: Varje test använder sin egen InMemory-databas
- **Beskrivande namn**: Testnamn beskriver vad som testas och förväntat resultat
- **FluentAssertions**: Använder FluentAssertions för mer läsbara assertions

## Exempel på testkod

```csharp
[Fact]
public async Task CanAddJobApplicationToDatabase()
{
    // Arrange
    using var context = CreateInMemoryContext();
    var jobApp = new JobApplication
    {
        Foretag = "Microsoft",
        Jobbtitel = "Software Engineer",
     UserId = "test-user-123"
    };

    // Act
    context.JobApplications.Add(jobApp);
    await context.SaveChangesAsync();

    // Assert
    var savedJobApp = await context.JobApplications.FirstOrDefaultAsync();
savedJobApp.Should().NotBeNull();
    savedJobApp!.Foretag.Should().Be("Microsoft");
}
```

## Lägga till fler tester

1. Skapa en ny testklass i `Tests/`-mappen
2. Använd `[Fact]` för enkla tester eller `[Theory]` med `[InlineData]` för parametriserade tester
3. Följ AAA-mönstret (Arrange-Act-Assert)
4. Använd FluentAssertions för assertions
5. Kör testerna för att verifiera att de fungerar

## Kodtäckning

För att se kodtäckning, kör:
```powershell
dotnet test Tests\JobApplicationTrackerV2.Tests.csproj /p:CollectCoverage=true
```

## Felsökning

Om testerna misslyckas:
1. Kontrollera att alla NuGet-paket är installerade: `dotnet restore`
2. Rensa och bygg om: `dotnet clean && dotnet build`
3. Kör testerna individuellt för att isolera problemet
4. Kontrollera att `Tests\**` är exkluderad från huvudprojektet i `JobApplicationTrackerV2.csproj`

## Licenser

- FluentAssertions använder Xceed Community License (gratis för icke-kommersiellt bruk)
