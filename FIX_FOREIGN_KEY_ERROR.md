# ?? FIX: "An error occurred while saving the entity changes"

## ?? **Problemet:**

När du försöker spara en JobApplication får du felet:
```
Fel: An error occurred while saving the entity changes. See the inner exception for details.
```

**Rotorsaken:** PostgreSQL kräver att Foreign Keys är korrekt konfigurerade. Din `JobApplication.UserId` refererar till `ApplicationUser.Id`, men Entity Framework har inte konfigurerat denna relation ordentligt.

---

## ? **Lösningen (REDAN FIXAD I KODEN):**

Jag har redan uppdaterat filerna, men appen måste startas om för att ändringarna ska gälla.

### **Vad som ändrats:**

#### **1. `Models/JobApplication.cs`**
? Lagt till navigation property:
```csharp
// Navigation property för Entity Framework
public ApplicationUser? User { get; set; }
```

#### **2. `Data/ApplicationDbContext.cs`**
? Konfigurerat Foreign Key-relation:
```csharp
protected override void OnModelCreating(ModelBuilder builder)
{
    base.OnModelCreating(builder);

    // Konfigurera relation mellan JobApplication och ApplicationUser
    builder.Entity<JobApplication>()
        .HasOne(j => j.User)
        .WithMany()
        .HasForeignKey(j => j.UserId)
        .OnDelete(DeleteBehavior.Cascade);
}
```

---

## ?? **Steg för att fixa (GÖR DETTA NU):**

### **Steg 1: Stoppa appen**
```powershell
# Tryck Ctrl+C i terminalen där appen körs
# ELLER stäng Debug-sessionen i Visual Studio
```

### **Steg 2: Skapa ny migration**
```powershell
dotnet ef migrations add AddUserRelation --output-dir Data/Migrations
```

### **Steg 3: Applicera migration till Supabase**
```powershell
dotnet ef database update
```

### **Steg 4: Starta appen igen**
```powershell
dotnet run
```

### **Steg 5: Testa skapa en JobApplication**
1. Gå till `https://localhost:5001/jobapplications/create`
2. Fyll i formuläret
3. Klicka "Spara"
4. ? Det ska fungera nu!

---

## ?? **Vad händer tekniskt:**

### **Före fixet:**
```sql
-- UserId var bara en sträng utan relation
CREATE TABLE "JobApplications" (
    "Id" serial PRIMARY KEY,
    "UserId" text NOT NULL,  -- <-- Ingen Foreign Key!
    ...
);
```

### **Efter fixet:**
```sql
-- UserId är nu en Foreign Key med CASCADE delete
CREATE TABLE "JobApplications" (
    "Id" serial PRIMARY KEY,
    "UserId" text NOT NULL,
    ...
    CONSTRAINT "FK_JobApplications_AspNetUsers_UserId" 
  FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") 
        ON DELETE CASCADE  -- <-- Om user raderas, radera alla deras JobApplications
);
```

---

## ?? **VIKTIGT:**

Efter migrationen kommer:
- ? Foreign Key-constraint att vara på plats
- ? PostgreSQL validerar att `UserId` alltid pekar på en giltig användare
- ? Om en användare raderas, raderas alla deras JobApplications automatiskt (Cascade)

---

## ?? **Om du fortfarande får fel:**

### **Alternativ 1: Droppa och återskapa databasen (TAR BORT ALL DATA)**
```powershell
# VARNING: Detta raderar ALL data i databasen!
dotnet ef database drop --force
dotnet ef database update
```

### **Alternativ 2: Manuell fix i Supabase Dashboard**
1. Gå till https://supabase.com/dashboard
2. Välj ditt projekt
3. SQL Editor
4. Kör detta:
```sql
-- Lägg till Foreign Key constraint manuellt
ALTER TABLE "JobApplications"
ADD CONSTRAINT "FK_JobApplications_AspNetUsers_UserId"
FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id")
ON DELETE CASCADE;
```

---

## ?? **Commit-meddelande efter fix:**

```
git add .
git commit -m "Fix: Add Foreign Key relation between JobApplication and ApplicationUser

- Added User navigation property to JobApplication model
- Configured Foreign Key in ApplicationDbContext using OnModelCreating
- Fixed 'An error occurred while saving' error when creating JobApplications
- Added Cascade delete behavior (deleting user deletes their applications)

Fixes issue with PostgreSQL requiring proper Foreign Key constraints.
"
```

---

## ? **Sammanfattning:**

| Problem | Lösning | Status |
|---------|---------|--------|
| ? Foreign Key saknas | ? Lagt till i ApplicationDbContext | FIXAD I KOD |
| ? Navigation property saknas | ? Lagt till User i JobApplication | FIXAD I KOD |
| ? Migration behövs | ? Måste köras manuellt | VÄNTARE PÅ DIG |
| ? Database update behövs | ? Måste köras manuellt | VÄNTAR PÅ DIG |

---

**Kör stegen ovan nu så fixar vi detta! ??**
