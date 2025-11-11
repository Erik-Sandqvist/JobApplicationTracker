# ?? Snabbstart - Databas för JobApplicationTracker

## ?? GRATIS Alternativ (Rekommenderat!)

### SQLite - 100% Gratis, Ingen Server

```powershell
# Kör automatiskt setup-script
.\setup-sqlite.ps1
```

**Fördelar:**
- ? Helt gratis (0 kr/månad)
- ? Ingen server behövs
- ? En fil = hela databasen
- ? Perfekt för 1-100 användare
- ? Enkelt att göra backup (kopiera fil)

**Se också:** [GRATIS_DATABAS.md](GRATIS_DATABAS.md) för fler gratis alternativ

---

## ?? Betalda Alternativ

### Azure SQL Database

**VARNING:** Detta kostar från ~45 kr/månad efter gratis trial!

```powershell
# Kör setup-script
.\setup-database.ps1
```

### För egen SQL Server:

```powershell
# Kör setup-script
.\setup-database.ps1
```

---

## ?? Jämförelse

| Databas | Kostnad | Setup | Lämplig för |
|---------|---------|-------|-------------|
| **SQLite** | **0 kr** | **5 min** | **1-100 användare** |
| LocalDB | 0 kr | Inkluderad | Endast utveckling |
| Azure SQL | Från 45 kr/mån | 10 min | 100+ användare |
| Supabase | 0-500 MB gratis | 15 min | Molnbaserade appar |

---

## ?? Rekommendation

**För personliga projekt / småskaliga appar:**
? Använd **SQLite** (helt gratis!)

**För produktion med många användare:**
? Överväg betald lösning när du har 100+ samtidiga användare

---

## Manuell Setup

### SQLite (Gratis):

```powershell
# 1. Installera paket
dotnet add package Microsoft.EntityFrameworkCore.Sqlite

# 2. Sätt connection string
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Data Source=jobapplications.db"

# 3. Uppdatera Program.cs (se SQLITE_KONVERTERING.md)
# Ändra: .UseSqlServer() ? .UseSqlite()

# 4. Kör migrationer
dotnet ef migrations add InitialCreate
dotnet ef database update

# 5. Starta appen
dotnet run
```

### Azure SQL Database (Betald):

```powershell
# 1. Sätt connection string
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=tcp:YOURSERVER.database.windows.net,1433;Initial Catalog=YOURDATABASE;User ID=USERNAME;Password=PASSWORD;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;"

# 2. Kör migrationer
dotnet ef database update

# 3. Starta appen
dotnet run
```

---

## Viktiga Säkerhetstips

? **GÖR INTE:**
- Spara lösenord i appsettings.json
- Commita connection strings till Git
- Commita databasfiler (.db) till Git

? **GÖR:**
- Använd User Secrets för utveckling
- Använd Environment Variables för produktion
- Gör regelbundna backups

---

## Felsökning

**"Cannot open database"**
? Kör: `dotnet ef database update`

**"Login failed"**
? Kontrollera användarnamn och lösenord

**"Network error"**
? Kontrollera brandväggsregler och server-namn

**"SQLite Error 1: no such table"**
? Kör: `dotnet ef database update`

---

## Mer Information

- **Gratis alternativ:** [GRATIS_DATABAS.md](GRATIS_DATABAS.md)
- **SQLite-konvertering:** [SQLITE_KONVERTERING.md](SQLITE_KONVERTERING.md)
- **Detaljerad guide:** [DATABAS_MIGRATION.md](DATABAS_MIGRATION.md)
