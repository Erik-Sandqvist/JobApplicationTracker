# Migrera till Riktig Databas - Guide

## ?? VIKTIGT: Kostnader

### ?? GRATIS Alternativ:
- ? **SQLite** - 0 kr/månad (Rekommenderat!)
- ? **Supabase** - 0 kr/månad (upp till 500MB)
- ? **PlanetScale** - 0 kr/månad (upp till 5GB)

### ?? BETALDA Alternativ:
- ? **Azure SQL Database** - Från ~45 kr/månad
- ? **Dedikerad SQL Server** - Från ~100 kr/månad (hosting)

**?? Se [GRATIS_DATABAS.md](GRATIS_DATABAS.md) för gratis alternativ!**

---

## Steg 1: Välj Databas

### Alternativ A: SQLite (100% GRATIS - REKOMMENDERAT)

**Kör setup-script:**
```powershell
.\setup-sqlite.ps1
```

**Fördelar:**
- Helt gratis
- Ingen server behövs
- Perfekt för 1-100 användare
- Enkel backup (kopiera fil)

**Nackdelar:**
- Mindre skalbart än SQL Server
- En användare kan skriva åt gången

---

### Alternativ B: Azure SQL Database (BETALD - ~45 kr/mån)

?? **VARNING: Detta kostar pengar!**

**Gratis trial:**
- 12 månader gratis med begränsningar
- Kräver kreditkort
- Kan debiteras automatiskt efter trial

**Efter gratis period:**
- Basic: ~45 kr/månad
- Standard: ~150 kr/månad

1. **Skapa Azure SQL Database:**
   - Gå till [Azure Portal](https://portal.azure.com)
   - Skapa ny "SQL Database"
   - Välj prisnivå
   - ?? **NOTERA:** Gratis trial är begränsad till 12 månader!

2. **Hämta Connection String:**
   ```
   Server=tcp:<yourserver>.database.windows.net,1433;
   Initial Catalog=<yourdatabase>;
   Persist Security Info=False;
   User ID=<yourusername>;
   Password=<yourpassword>;
   MultipleActiveResultSets=False;
   Encrypt=True;
   TrustServerCertificate=False;
   Connection Timeout=30;
   ```

### Alternativ C: Dedikerad SQL Server (BETALD - från ~100 kr/mån)

?? **VARNING: Detta kostar pengar!**

1. **Installera SQL Server** (om inte redan installerad)
2. **Skapa databas:**
   ```sql
   CREATE DATABASE JobApplicationTrackerV2;
   ```

3. **Connection String:**
   ```
   Server=<server-ip-or-hostname>;
   Database=JobApplicationTrackerV2;
User Id=<username>;
   Password=<password>;
   MultipleActiveResultSets=true;
   TrustServerCertificate=True;
   ```

---

## Steg 2: Konfigurera Connection String SÄKERT

### Metod 1: User Secrets (Utveckling - REKOMMENDERAT)

**Steg 1:** Initiera User Secrets
```powershell
cd C:\dev\GitHub\JobApplicationTrackerV2
dotnet user-secrets init
```

**Steg 2:** Lägg till connection string
```powershell
# För Azure SQL Database:
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=tcp:yourserver.database.windows.net,1433;Initial Catalog=yourdatabase;User ID=yourusername;Password=yourpassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;"

# För egen SQL Server:
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=yourserver;Database=JobApplicationTrackerV2;User Id=yourusername;Password=yourpassword;MultipleActiveResultSets=true;TrustServerCertificate=True;"
```

**Steg 3:** Lista secrets (för att verifiera)
```powershell
dotnet user-secrets list
```

### Metod 2: Environment Variables (Produktion)

**Windows:**
```powershell
$env:ConnectionStrings__DefaultConnection="din-connection-string-här"
```

**Linux/Mac:**
```bash
export ConnectionStrings__DefaultConnection="din-connection-string-här"
```

### Metod 3: Azure App Service Configuration (För Azure hosting)

1. Gå till din App Service i Azure Portal
2. Settings ? Configuration
3. Application settings ? New application setting
4. Name: `ConnectionStrings__DefaultConnection`
5. Value: Din connection string

---

## Steg 3: Uppdatera Databas-schema

**Kör migrationer mot den nya databasen:**

```powershell
# Ta bort gamla migration-filer (VALFRITT - endast om du vill börja om)
# Remove-Item -Path "Data\Migrations\*" -Force

# Skapa ny migration
dotnet ef migrations add InitialCreate

# Applicera migration till databasen
dotnet ef database update
```

**Om du får fel:**
```powershell
# Installera EF Core tools om de saknas
dotnet tool install --global dotnet-ef

# Uppdatera till senaste version
dotnet tool update --global dotnet-ef
```

---

## Steg 4: Testa Anslutningen

**Kör applikationen:**
```powershell
dotnet run
```

**Verifiera:**
1. Öppna applikationen i webbläsaren
2. Logga in eller registrera ett konto
3. Skapa en testansökan
4. Kontrollera att data sparas i den nya databasen

---

## Steg 5: Säkerhet (VIKTIGT!)

### ? GÖR:
- ? Använd User Secrets för utveckling
- ? Använd Environment Variables eller Azure Configuration för produktion
- ? Använd starka lösenord
- ? Aktivera SSL/TLS (Encrypt=True)
- ? Begränsa databasåtkomst till endast din app-server

### ? GÖR INTE:
- ? Commit connection strings med lösenord till Git
- ? Spara lösenord i klartext i appsettings.json
- ? Använd samma lösenord för utveckling och produktion
- ? Öppna databasen för publik åtkomst

---

## Steg 6: Backup-strategi

### Azure SQL Database:
- Automatiska backups ingår
- Konfigurera retention period i Azure Portal

### Egen SQL Server:
```sql
-- Skapa backup
BACKUP DATABASE JobApplicationTrackerV2
TO DISK = 'C:\Backups\JobApplicationTrackerV2.bak'
WITH FORMAT, COMPRESSION;

-- Återställ backup
RESTORE DATABASE JobApplicationTrackerV2
FROM DISK = 'C:\Backups\JobApplicationTrackerV2.bak'
WITH REPLACE;
```

---

## Felsökning

### Problem: "Cannot open database"
**Lösning:** Kör `dotnet ef database update` för att skapa tabeller

### Problem: "Login failed for user"
**Lösning:** Kontrollera användarnamn och lösenord i connection string

### Problem: "A network-related or instance-specific error"
**Lösning:** 
- Kontrollera server-namn/IP
- Kontrollera brandväggsregler
- För Azure: Lägg till din IP-adress i Azure Firewall

### Problem: Migrationer fungerar inte
**Lösning:**
```powershell
# Ta bort gamla migrationer
Remove-Item -Path "Data\Migrations\*" -Recurse -Force

# Skapa ny initial migration
dotnet ef migrations add InitialCreate

# Applicera
dotnet ef database update
```

---

## Exempel: Komplett Azure SQL Setup

```powershell
# 1. Initiera user secrets
dotnet user-secrets init

# 2. Sätt connection string (byt ut värden)
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=tcp:myserver.database.windows.net,1433;Initial Catalog=JobAppTrackerDB;User ID=adminuser;Password=StrongPassword123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;"

# 3. Kör migrationer
dotnet ef database update

# 4. Starta appen
dotnet run
```

---

## Checklista

- [ ] Databas skapad
- [ ] Connection string konfigurerad säkert (User Secrets)
- [ ] Migrationer körda
- [ ] Testansökan skapad
- [ ] Data syns i databasen
- [ ] Brandväggsregler konfigurerade (för remote databas)
- [ ] Backup-strategi på plats
- [ ] Connection string INTE i Git

---

## Nästa steg

När du är redo för produktion:
1. Konfigurera connection string via Environment Variables
2. Aktivera SSL för databas-anslutning
3. Implementera proper error handling
4. Sätt upp monitoring och alerting
5. Planera backup-schema
