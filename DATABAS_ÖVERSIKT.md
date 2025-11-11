# ?? Databas-alternativ för JobApplicationTracker

## ?? Vill du ha GRATIS? ? SQLite

**Snabbast:** Kör detta script
```powershell
.\setup-sqlite.ps1
```

**Manuellt:**
1. Installera: `dotnet add package Microsoft.EntityFrameworkCore.Sqlite`
2. Ändra Program.cs: `.UseSqlServer()` ? `.UseSqlite()`
3. Sätt connection string: `Data Source=jobapplications.db`
4. Kör migrationer: `dotnet ef database update`

**Läs mer:** [GRATIS_DATABAS.md](GRATIS_DATABAS.md)

---

## ?? Vill du ha Molnbaserad? ? Betald Lösning

**Azure SQL Database** (från ~45 kr/månad):
```powershell
.\setup-database.ps1
```

**Läs mer:** [DATABAS_MIGRATION.md](DATABAS_MIGRATION.md)

---

## ?? Alla Guider

| Guide | Beskrivning |
|-------|-------------|
| [GRATIS_DATABAS.md](GRATIS_DATABAS.md) | Alla gratis alternativ |
| [DATABAS_QUICKSTART.md](DATABAS_QUICKSTART.md) | Snabb referens |
| [SQLITE_KONVERTERING.md](SQLITE_KONVERTERING.md) | Konvertera till SQLite |
| [DATABAS_MIGRATION.md](DATABAS_MIGRATION.md) | Detaljerad guide för alla databaser |

---

## ? Snabbjämförelse

### SQLite (Gratis)
- ? 0 kr/månad
- ? 5 minuters setup
- ? Perfekt för < 100 användare
- ? Enkla backups
- ? Begränsad skalbarhet

### Azure SQL (Betald)
- ? Från 45 kr/månad
- ? Automatiska backups
- ? Hög tillgänglighet
- ? Skalbart
- ? Kräver kreditkort

---

## ?? Rekommendation

**Personligt projekt?** ? SQLite (gratis)
**Företag med < 100 användare?** ? SQLite (gratis)
**Stora företag?** ? Azure SQL (betald)

**99% av fallen:** SQLite är tillräckligt! ??
