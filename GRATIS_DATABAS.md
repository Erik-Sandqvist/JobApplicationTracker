# ?? Gratis Databas-alternativ för JobApplicationTracker

## Rekommenderade Gratis Alternativ

### 1. **SQLite (Enklast - REKOMMENDERAT för små projekt)**

? **Fördelar:**
- Helt gratis
- Ingen server behövs
- En fil = hela databasen
- Perfekt för utveckling och små appar
- Fungerar direkt utan konfiguration

? **Nackdelar:**
- Inte lika skalbart som SQL Server
- Begränsad samtidig skrivåtkomst
- Mindre funktioner än SQL Server

#### **Setup SQLite:**

**Steg 1:** Installera NuGet-paket
```powershell
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

**Steg 2:** Uppdatera Program.cs (jag gör detta automatiskt nedan)

**Steg 3:** Kör migrationer
```powershell
dotnet ef migrations add InitialCreate
dotnet ef database update
```

**Klart!** Databasen skapas som en fil: `jobapplications.db`

---

### 2. **PostgreSQL + Supabase (Gratis upp till 500MB)**

? **Fördelar:**
- 500MB gratis databas
- Molnbaserad (tillgänglig överallt)
- Automatiska backups
- Bra prestanda

? **Nackdelar:**
- Kräver registrering
- Begränsningar i gratisversionen

#### **Setup Supabase:**

1. Gå till [supabase.com](https://supabase.com)
2. Skapa gratis konto
3. Skapa nytt projekt
4. Kopiera PostgreSQL connection string
5. Använd setup-scriptet nedan

---

### 3. **MySQL + PlanetScale (Gratis upp till 5GB)**

? **Fördelar:**
- 5GB gratis
- Molnbaserad
- Bra för produktion
- Automatisk skalning

? **Nackdelar:**
- Kräver registrering
- Vissa begränsningar

---

### 4. **Azure SQL Database - Gratis Tier** ??

?? **VIKTIGT:** Azure SQL har INGEN permanent gratis tier!

**Azure Free Trial:**
- 12 månader gratis (med begränsningar)
- Kräver kreditkort (men debiteras inte utan godkännande)
- 200 USD i credits första månaden

**Efter gratis period:**
- Basic tier: ~5 USD/månad
- Kan stängas av för att undvika kostnader

---

## ?? Rekommendation för Gratis Användning

### För Utveckling/Testning:
? **SQLite** (helt gratis, inga begränsningar)

### För Produktion (Liten app):
? **SQLite** (fungerar bra för 1-100 användare)

### För Produktion (Många användare):
? **Supabase** eller **PlanetScale** (gratis tier)

---

## ?? Snabbstart: SQLite (100% Gratis)

### Automatisk Setup:

```powershell
# 1. Installera SQLite-paketet
dotnet add package Microsoft.EntityFrameworkCore.Sqlite

# 2. Använd SQLite istället för SQL Server (kör setup-scriptet)
.\setup-sqlite.ps1

# 3. Kör migrationer
dotnet ef database update

# 4. Starta appen
dotnet run
```

**KLART!** Din databas är nu en lokal fil och kostar 0 kr! ??

---

## ?? SQLite vs SQL Server - Jämförelse

| Funktion | SQLite | SQL Server |
|----------|--------|------------|
| **Kostnad** | Gratis | LocalDB: Gratis<br>Azure: Från 5 USD/mån |
| **Installation** | Ingen server | LocalDB: Inkluderad<br>Full: Installation krävs |
| **Databasfil** | En fil (.db) | Flera filer (.mdf, .ldf) |
| **Max storlek** | 281 TB | Praktiskt obegränsat |
| **Samtidiga användare** | Begränsat | Tusentals |
| **Backup** | Kopiera fil | SQL Server backup |
| **Hosting** | Ingår i app | Behöver separat server |
| **Lämplig för** | 1-100 användare | 100+ användare |

---

## ?? Kostnadsjämförelse (per månad)

| Databas | Kostnad | Storage |
|---------|---------|---------|
| **SQLite** | **0 kr** | Begränsad av disk |
| **Supabase Free** | **0 kr** | 500 MB |
| **PlanetScale Free** | **0 kr** | 5 GB |
| **Azure SQL Basic** | ~45 kr | 2 GB |
| **Azure SQL Standard** | ~150 kr | 250 GB |

---

## ?? Migrera från SQL Server till SQLite

### Om du redan använder LocalDB:

```powershell
# 1. Installera SQLite
dotnet add package Microsoft.EntityFrameworkCore.Sqlite

# 2. Uppdatera connection string (se setup-sqlite.ps1)

# 3. Skapa nya migrationer för SQLite
dotnet ef migrations add InitialCreate -c ApplicationDbContext

# 4. Skapa databasen
dotnet ef database update
```

**OBS:** Du förlorar din befintliga data. Exportera först om du vill behålla den!

---

## ? När ska du betala för databas?

Betala för databas när:
- ? Du har 100+ samtidiga användare
- ? Du behöver garanterad uptime (99.99%)
- ? Du behöver automatiska backups
- ? Du behöver geo-replikering
- ? Du tjänar pengar på appen

**För hobby/personliga projekt:**
? SQLite är perfekt och helt gratis! ??

---

## ??? Backup för SQLite (Gratis metoder)

### Metod 1: Kopiera fil
```powershell
Copy-Item jobapplications.db jobapplications_backup.db
```

### Metod 2: Git LFS (för större databaser)
```bash
git lfs install
git lfs track "*.db"
git add .gitattributes
```

### Metod 3: Cloud Backup (OneDrive/Google Drive)
- Kopiera .db-filen till molnlagring
- Automatisk backup med molntjänstens synk

---

## ?? Sammanfattning

För **100% gratis** lösning:
1. Använd **SQLite** (rekommenderat)
2. Eller **Supabase**/PlanetScale gratis tier

SQLite funkar perfekt för:
- ? Personliga projekt
- ? Små företagsappar (< 100 användare)
- ? Prototyper
- ? Utveckling/testning

Du behöver INTE betala för databas förrän du har tusentals användare! ??
