# Setup SQLite - 100% Gratis Databas
# Konverterar projektet till att använda SQLite istället för SQL Server

Write-Host "=== SQLite Setup - Gratis Databas ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "SQLite är helt gratis och kräver ingen server!" -ForegroundColor Green
Write-Host ""

# Kontrollera om SQLite-paketet är installerat
Write-Host "Kontrollerar om SQLite-paket är installerat..." -ForegroundColor Yellow

$packageInstalled = dotnet list package | Select-String "Microsoft.EntityFrameworkCore.Sqlite"

if (-not $packageInstalled) {
    Write-Host "Installerar SQLite-paket..." -ForegroundColor Yellow
    dotnet add package Microsoft.EntityFrameworkCore.Sqlite
    Write-Host "SQLite-paket installerat!" -ForegroundColor Green
} else {
    Write-Host "SQLite-paket är redan installerat." -ForegroundColor Green
}

Write-Host ""
Write-Host "Uppdaterar connection string för SQLite..." -ForegroundColor Yellow

# SQLite connection string
$sqliteConnection = "Data Source=jobapplications.db"

# Uppdatera User Secrets
dotnet user-secrets init 2>$null
dotnet user-secrets set "ConnectionStrings:DefaultConnection" $sqliteConnection

Write-Host "Connection string uppdaterad!" -ForegroundColor Green
Write-Host ""

Write-Host "=== Viktigt: Uppdatera Program.cs ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "Du måste ändra en rad i Program.cs:" -ForegroundColor Yellow
Write-Host ""
Write-Host "FÖRE:" -ForegroundColor Red
Write-Host "  .UseSqlServer(connectionString)" -ForegroundColor White
Write-Host ""
Write-Host "EFTER:" -ForegroundColor Green
Write-Host ".UseSqlite(connectionString)" -ForegroundColor White
Write-Host ""

Write-Host "Vill du att jag öppnar Program.cs nu? (J/N)" -ForegroundColor Cyan
$openFile = Read-Host

if ($openFile -eq "J" -or $openFile -eq "j") {
  code Program.cs
    Write-Host ""
    Write-Host "Öppnade Program.cs. Ändra båda ställena där UseSqlServer används!" -ForegroundColor Yellow
    Write-Host "Tryck Enter när du är klar..." -ForegroundColor Cyan
    Read-Host
}

Write-Host ""
Write-Host "Vill du ta bort gamla migrationer och skapa nya för SQLite? (J/N)" -ForegroundColor Cyan
Write-Host "(Rekommenderat om du byter från SQL Server)" -ForegroundColor Yellow
$recreateMigrations = Read-Host

if ($recreateMigrations -eq "J" -or $recreateMigrations -eq "j") {
    Write-Host ""
    Write-Host "Tar bort gamla migrationer..." -ForegroundColor Yellow
    
    if (Test-Path "Data\Migrations") {
        Remove-Item -Path "Data\Migrations\*" -Recurse -Force -ErrorAction SilentlyContinue
      Write-Host "Gamla migrationer borttagna." -ForegroundColor Green
    }
    
    Write-Host ""
    Write-Host "Skapar nya migrationer för SQLite..." -ForegroundColor Yellow
    
    try {
        dotnet ef migrations add InitialCreate
     Write-Host "Migrationer skapade!" -ForegroundColor Green
    }
    catch {
        Write-Host "Fel vid skapande av migrationer: $_" -ForegroundColor Red
        Write-Host "Se till att Program.cs är uppdaterad!" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "Vill du skapa databasen nu? (J/N)" -ForegroundColor Cyan
$createDb = Read-Host

if ($createDb -eq "J" -or $createDb -eq "j") {
    Write-Host ""
    Write-Host "Skapar SQLite-databas..." -ForegroundColor Yellow
    
    try {
        dotnet ef database update
        Write-Host ""
        Write-Host "Databas skapad: jobapplications.db" -ForegroundColor Green
     
        # Visa filstorlek
        if (Test-Path "jobapplications.db") {
            $fileSize = (Get-Item "jobapplications.db").Length / 1KB
            Write-Host "Databasstorlek: $([math]::Round($fileSize, 2)) KB" -ForegroundColor Cyan
        }
    }
    catch {
        Write-Host "Fel vid skapande av databas: $_" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "=== Setup Slutförd! ===" -ForegroundColor Green
Write-Host ""
Write-Host "Din app använder nu SQLite - helt gratis!" -ForegroundColor Green
Write-Host ""
Write-Host "Databas-fil: jobapplications.db" -ForegroundColor Cyan
Write-Host "Kostnad: 0 kr/månad" -ForegroundColor Green
Write-Host ""
Write-Host "Nästa steg:" -ForegroundColor Cyan
Write-Host "1. Starta appen: dotnet run" -ForegroundColor White
Write-Host "2. Registrera ett konto och testa!" -ForegroundColor White
Write-Host ""
Write-Host "Backup:" -ForegroundColor Cyan
Write-Host "  Kopiera jobapplications.db till säker plats regelbundet" -ForegroundColor White
Write-Host ""

Write-Host "Vill du starta applikationen nu? (J/N)" -ForegroundColor Cyan
$startApp = Read-Host

if ($startApp -eq "J" -or $startApp -eq "j") {
    Write-Host ""
    Write-Host "Startar applikationen..." -ForegroundColor Yellow
    Write-Host ""
    dotnet run
}

Write-Host ""
Write-Host "Tryck Enter för att avsluta..."
Read-Host
