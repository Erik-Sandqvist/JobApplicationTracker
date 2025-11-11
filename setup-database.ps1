# Migrera till Produktionsdatabas - Setup Script
# Detta script hjälper dig att konfigurera databas-anslutning säkert

Write-Host "=== JobApplicationTracker - Databas Migration ===" -ForegroundColor Cyan
Write-Host ""

# Kontrollera om dotnet ef är installerat
Write-Host "Kontrollerar om EF Core tools är installerat..." -ForegroundColor Yellow
$efInstalled = dotnet tool list -g | Select-String "dotnet-ef"

if (-not $efInstalled) {
    Write-Host "EF Core tools saknas. Installerar..." -ForegroundColor Yellow
    dotnet tool install --global dotnet-ef
    Write-Host "EF Core tools installerat!" -ForegroundColor Green
} else {
    Write-Host "EF Core tools är redan installerat." -ForegroundColor Green
}

Write-Host ""
Write-Host "Välj databas-typ:" -ForegroundColor Cyan
Write-Host "1. Azure SQL Database"
Write-Host "2. Dedikerad SQL Server"
Write-Host "3. Avbryt"
Write-Host ""

$choice = Read-Host "Ange ditt val (1-3)"

switch ($choice) {
    "1" {
        Write-Host ""
        Write-Host "=== Azure SQL Database Setup ===" -ForegroundColor Cyan
      Write-Host ""
        
        $serverName = Read-Host "Ange Azure SQL Server namn (utan .database.windows.net)"
  $databaseName = Read-Host "Ange databas-namn"
        $username = Read-Host "Ange admin användarnamn"
        $password = Read-Host "Ange lösenord" -AsSecureString
    $passwordPlain = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($password))
        
 $connectionString = "Server=tcp:$serverName.database.windows.net,1433;Initial Catalog=$databaseName;Persist Security Info=False;User ID=$username;Password=$passwordPlain;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
        
        Write-Host ""
        Write-Host "Sparar connection string i User Secrets..." -ForegroundColor Yellow
   
     # Initiera user secrets om inte redan gjort
        dotnet user-secrets init
        
        # Spara connection string
dotnet user-secrets set "ConnectionStrings:DefaultConnection" $connectionString
      
        Write-Host "Connection string sparad!" -ForegroundColor Green
    }
    
    "2" {
        Write-Host ""
        Write-Host "=== Dedikerad SQL Server Setup ===" -ForegroundColor Cyan
        Write-Host ""
        
  $serverHost = Read-Host "Ange server IP eller hostname"
  $databaseName = Read-Host "Ange databas-namn (tryck Enter för 'JobApplicationTrackerV2')"
   if ([string]::IsNullOrWhiteSpace($databaseName)) {
        $databaseName = "JobApplicationTrackerV2"
        }
        $username = Read-Host "Ange SQL användarnamn"
   $password = Read-Host "Ange lösenord" -AsSecureString
 $passwordPlain = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($password))
     
        $connectionString = "Server=$serverHost;Database=$databaseName;User Id=$username;Password=$passwordPlain;MultipleActiveResultSets=true;Encrypt=True;TrustServerCertificate=False;"
        
   Write-Host ""
      Write-Host "Sparar connection string i User Secrets..." -ForegroundColor Yellow
        
  # Initiera user secrets om inte redan gjort
        dotnet user-secrets init
        
      # Spara connection string
        dotnet user-secrets set "ConnectionStrings:DefaultConnection" $connectionString
        
        Write-Host "Connection string sparad!" -ForegroundColor Green
  }
    
    "3" {
  Write-Host "Avbryter..." -ForegroundColor Yellow
        exit
    }
    
    default {
        Write-Host "Ogiltigt val. Avbryter..." -ForegroundColor Red
        exit
    }
}

Write-Host ""
Write-Host "Vill du köra databas-migrationer nu? (J/N)" -ForegroundColor Cyan
$runMigrations = Read-Host

if ($runMigrations -eq "J" -or $runMigrations -eq "j") {
    Write-Host ""
    Write-Host "Kör databas-migrationer..." -ForegroundColor Yellow
    
try {
        dotnet ef database update
        Write-Host ""
        Write-Host "Migrationer slutförda framgångsrikt!" -ForegroundColor Green
        
        Write-Host ""
        Write-Host "Vill du starta applikationen? (J/N)" -ForegroundColor Cyan
$startApp = Read-Host
        
        if ($startApp -eq "J" -or $startApp -eq "j") {
         Write-Host ""
 Write-Host "Startar applikationen..." -ForegroundColor Yellow
            Write-Host ""
    dotnet run
        }
    }
    catch {
        Write-Host ""
        Write-Host "Fel vid migrering: $_" -ForegroundColor Red
      Write-Host ""
        Write-Host "Försök köra manuellt:" -ForegroundColor Yellow
    Write-Host "dotnet ef database update" -ForegroundColor White
    }
} else {
    Write-Host ""
  Write-Host "=== Setup slutförd! ===" -ForegroundColor Green
    Write-Host ""
Write-Host "Nästa steg:" -ForegroundColor Cyan
    Write-Host "1. Kör migrationer: dotnet ef database update" -ForegroundColor White
    Write-Host "2. Starta appen: dotnet run" -ForegroundColor White
    Write-Host ""
}

Write-Host "Tryck Enter för att avsluta..."
Read-Host
