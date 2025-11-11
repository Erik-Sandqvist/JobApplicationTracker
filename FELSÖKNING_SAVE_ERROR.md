# ?? Felsökning: "An error occurred while saving the entity changes"

## ? **Vad vi har fixat:**

### **1. Foreign Key-relation tillagd**
- ? Migration `AddUserRelation` skapad och applicerad
- ? Foreign Key constraint `FK_JobApplications_AspNetUsers_UserId` finns i databasen
- ? Index `IX_JobApplications_UserId` skapad för prestanda

### **2. Förbättrad error-hantering**
- ? Visar nu `InnerException.Message` istället för bara `Message`
- ? Loggar detaljer till konsolen
- ? Validerar att användaren finns innan sparning
- ? Validerar att UserId är satt

---

## ?? **Testa igen nu:**

### **Steg 1: Stoppa appen om den körs**
- Tryck **Ctrl+C** i terminalen ELLER
- Tryck **Shift+F5** i Visual Studio

### **Steg 2: Starta appen**
```powershell
dotnet run
```

ELLER i Visual Studio: Tryck **F5**

### **Steg 3: Testa skapa JobApplication**
1. Öppna: `https://localhost:5001/jobapplications/create`
2. Fyll i:
- **Företag:** Microsoft
   - **Jobbtitel:** Software Engineer  
   - **Plats:** Stockholm
   - **Ansökningsdatum:** (dagens datum)
   - **Status:** Väntar på svar
3. Klicka **"Spara"**

### **Steg 4: Vad ska hända**

**? OM DET FUNGERAR:**
- Grön success-notifikation: "Jobbansökan skapad!"
- Omdirigeras till `/jobapplications`
- Du ser din nya jobbansökan i listan

**? OM DET FEL UPPSTÅR:**
- Röd error-notifikation med **detaljerat felmeddelande**
- Konsolen visar **full stack trace**
- Läs felmeddelandet noga och rapportera det

---

## ?? **Kolla konsolloggar:**

När appen körs, titta på konsolen för dessa meddelanden:

```
info: Components.Pages.JobApplications.Create[0]
      UserId set to: abc123...
info: Components.Pages.JobApplications.Create[0]
Attempting to save JobApplication: {...}
info: Components.Pages.JobApplications.Create[0]
      JobApplication saved successfully
```

**OM FEL UPPSTÅR:**
```
fail: Components.Pages.JobApplications.Create[0]
      Database error while saving JobApplication: [DETALJERAT FELMEDDELANDE]
```

---

## ?? **Vanliga fel och lösningar:**

### **Fel 1: "Användare med ID ... finns inte!"**
**Orsak:** UserId pekar på en användare som inte finns i databasen

**Lösning:**
1. Logga ut
2. Registrera nytt konto
3. Logga in
4. Försök igen

---

### **Fel 2: "Ingen användare är inloggad!"**
**Orsak:** UserId är tom eller null

**Lösning:**
1. Logga ut
2. Logga in igen
3. Försök igen

---

### **Fel 3: "violates foreign key constraint"**
**Orsak:** UserId pekar på en ogilt användare

**Lösning:**
```powershell
# Verifiera att migrationen är applicerad
dotnet ef migrations list

# Om AddUserRelation INTE finns i listan, applicera den:
dotnet ef database update
```

---

### **Fel 4: "duplicate key value violates unique constraint"**
**Orsak:** Det finns redan en rad med samma ID (ovanligt)

**Lösning:**
```powershell
# Återställ ID-sekvensen i PostgreSQL
# Koppla till Supabase via SQL Editor och kör:
SELECT setval('JobApplications_Id_seq', (SELECT MAX("Id") FROM "JobApplications"));
```

---

## ?? **Verifiera i Supabase Dashboard:**

Efter lyckad sparning:

1. Gå till: https://supabase.com/dashboard
2. Välj ditt projekt
3. **Table Editor** ? **JobApplications**
4. Du borde se:
   - `Id` = 1 (eller högre)
   - `Foretag` = "Microsoft"
   - `Jobbtitel` = "Software Engineer"
   - `UserId` = (din användar-ID från AspNetUsers)
   - `AnsokanDatum` = dagens datum
   - `Status` = 0 (VantarPaSvar)

5. **Table Editor** ? **AspNetUsers**
6. Kopiera din användares `Id` (bör matcha `UserId` i JobApplications)

---

## ?? **Om inget fungerar:**

### **Drastisk lösning: Återskapa databasen**

?? **VARNING: Detta raderar ALL data!**

```powershell
# Droppa databasen
dotnet ef database drop --force

# Återskapa från scratch
dotnet ef database update
```

Detta skapar:
1. Alla Identity-tabeller (AspNetUsers, etc.)
2. JobApplications med korrekt Foreign Key
3. Alla index och constraints

Sedan:
1. Registrera nytt konto
2. Testa skapa JobApplication

---

## ?? **Rapportera fel:**

Om du fortfarande får fel, ge mig:

1. **Exakt felmeddelande** från röd notifikation
2. **Konsolloggar** (kopiera hela felet)
3. **Output från:**
   ```powershell
   dotnet ef migrations list
   ```

---

## ? **Checklist innan du testar:**

- [ ] Appen är stoppad
- [ ] `dotnet ef migrations list` visar `AddUserRelation`
- [ ] `dotnet run` startar utan fel
- [ ] Du är inloggad i appen
- [ ] Du har kört `dotnet build` utan errors

---

**Om allt är checkat, testa skapa en JobApplication nu! ??**
