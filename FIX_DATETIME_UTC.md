# ? FIXAT: DateTime UTC-problem med PostgreSQL

## ?? **Problemet:**

```
Databasfel: Cannot write DateTime with Kind=Local to PostgreSQL type 'timestamp with time zone', only UTC is supported.
```

**Orsak:** PostgreSQL med Npgsql-drivrutinen kräver att alla `DateTime`-värden ska vara i **UTC** (Coordinated Universal Time), inte lokal tid.

---

## ? **Lösningar (ALLA IMPLEMENTERADE):**

### **1. Konvertera DateTime till UTC i Create.razor**

```csharp
if (ansokanDatum.HasValue)
{
    // ? Konvertera till UTC
    model.AnsokanDatum = DateTime.SpecifyKind(ansokanDatum.Value, DateTimeKind.Utc);
}
else
{
    // Fallback till dagens datum i UTC
    model.AnsokanDatum = DateTime.UtcNow;
}
```

**Vad detta gör:**
- `DateTime.SpecifyKind()` konverterar ett befintligt DateTime till UTC
- Om inget datum anges, använd `DateTime.UtcNow` istället för `DateTime.Today`

---

### **2. Uppdatera JobApplication-modellen**

```csharp
// FÖRE (fel):
public DateTime AnsokanDatum { get; set; } = DateTime.Today; // ? Local time

// EFTER (rätt):
public DateTime AnsokanDatum { get; set; } = DateTime.UtcNow; // ? UTC
```

**Vad detta gör:**
- Alla nya `JobApplication`-instanser får automatiskt UTC-tid som default

---

### **3. Global Npgsql-konfiguration i Program.cs**

```csharp
// ? Aktivera "legacy timestamp behavior" för enklare hantering
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
```

**Vad detta gör:**
- Npgsql konverterar automatiskt ALL DateTime till UTC
- Du behöver inte tänka på UTC/Local i resten av koden
- Fungerar även för befintliga data

---

## ?? **Testa nu:**

### **1. Stoppa appen (om den körs)**
```powershell
Ctrl+C
```

### **2. Starta appen**
```powershell
dotnet run
```

### **3. Testa skapa JobApplication**
1. Gå till: `https://localhost:5001/jobapplications/create`
2. Fyll i formuläret
3. Klicka "Spara"
4. ? **Det ska fungera nu!**

---

## ?? **Hur fungerar DateTime i PostgreSQL:**

### **Skillnad mellan Local och UTC:**

| DateTime.Kind | Beskrivning | PostgreSQL-kompatibel? |
|--------------|-------------|----------------------|
| **Local** | Lokal tid (ex: svensk tid) | ? NEJ |
| **UTC** | Universal tid (samma överallt) | ? JA |
| **Unspecified** | Okänd tidszon | ?? Beror på konfiguration |

### **Exempel:**

```csharp
// ? Dessa fungerar INTE med PostgreSQL (utan legacy mode):
DateTime.Today    // Kind = Local
DateTime.Now    // Kind = Local
new DateTime(2025, 1, 1) // Kind = Unspecified

// ? Dessa fungerar:
DateTime.UtcNow  // Kind = UTC
DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc)
DateTime.Parse("2025-01-01").ToUniversalTime()
```

---

## ?? **Vad händer med tidszoner?**

### **När du sparar:**
```
User väljer datum: 2025-01-15 (lokal tid, Sverige)
    ?
Konverteras till UTC: 2025-01-14 23:00:00 (om vintertid)
    ?
Sparas i PostgreSQL: 2025-01-14 23:00:00+00
```

### **När du läser:**
```
PostgreSQL: 2025-01-14 23:00:00+00
?
C# får: DateTime med Kind = Utc
    ?
Visa i UI: Konvertera till lokal tid (om önskat)
```

### **För att visa lokal tid i UI:**

```csharp
// I Razor-komponenten:
@jobApp.AnsokanDatum.ToLocalTime().ToString("yyyy-MM-dd")
```

---

## ?? **Alternativa lösningar:**

### **Alternativ A: Använd DateOnly istället för DateTime**

Om du bara bryr dig om datum (inte tid):

```csharp
// I JobApplication.cs:
public DateOnly AnsokanDatum { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
```

**Fördelar:**
- Ingen tidszon-förvirring
- Tydligare intent (bara datum)
- Fungerar perfekt med PostgreSQL

**Nackdelar:**
- Kräver .NET 6+
- Migration behövs för att ändra kolumntyp

---

### **Alternativ B: Konfigurera EF Core att alltid konvertera**

```csharp
// I ApplicationDbContext.cs:
protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
{
  configurationBuilder
     .Properties<DateTime>()
        .HaveConversion<UtcValueConverter>();
}

public class UtcValueConverter : ValueConverter<DateTime, DateTime>
{
    public UtcValueConverter() : base(
    v => v.ToUniversalTime(),
    v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
    { }
}
```

**Fördelar:**
- Automatisk konvertering överallt
- Inget manuellt arbete

**Nackdelar:**
- Lite mer komplex kod

---

## ?? **Best Practices för DateTime med PostgreSQL:**

### **? GÖR:**
- Använd `DateTime.UtcNow` för nuvarande tid
- Använd `DateTime.SpecifyKind(..., DateTimeKind.Utc)` för konvertering
- Aktivera `EnableLegacyTimestampBehavior` för enklare hantering
- Överväg `DateOnly` för datum utan tid

### **? UNDVIK:**
- `DateTime.Now` (lokal tid)
- `DateTime.Today` (lokal tid)
- `new DateTime(...)` utan att specificera Kind

---

## ?? **Om du fortfarande får fel:**

### **Kontrollera befintlig data:**

Om du redan har data i databasen med lokal tid:

```sql
-- Koppla till Supabase SQL Editor och kör:
UPDATE "JobApplications"
SET "AnsokanDatum" = "AnsokanDatum" AT TIME ZONE 'Europe/Stockholm' AT TIME ZONE 'UTC'
WHERE "AnsokanDatum" IS NOT NULL;
```

---

### **Debug-tips:**

```csharp
// I OnValidSubmit:
Logger.LogInformation("DateTime Info: Value={Value}, Kind={Kind}", 
    model.AnsokanDatum, 
    model.AnsokanDatum.Kind);
```

Output bör visa:
```
DateTime Info: Value=2025-01-15 00:00:00, Kind=Utc
```

---

## ? **Sammanfattning:**

| Problem | Lösning | Status |
|---------|---------|--------|
| ? DateTime.Today = Local | ? Använd DateTime.UtcNow | FIXAD |
| ? MudDatePicker ger Local | ? Konvertera med SpecifyKind | FIXAD |
| ? PostgreSQL kräver UTC | ? EnableLegacyTimestampBehavior | FIXAD |

---

**Nu ska det fungera! Testa skapa en JobApplication igen! ??**
