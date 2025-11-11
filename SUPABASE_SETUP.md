# ?? Supabase Setup Guide

## ? **Vad vi har gjort hittills:**

1. ? Installerat `Npgsql.EntityFrameworkCore.PostgreSQL` (v9.0.0)
2. ? Uppdaterat `Microsoft.EntityFrameworkCore.Design` och `.Tools` (v9.0.0)
3. ? Tagit bort `Microsoft.EntityFrameworkCore.Sqlite`
4. ? Uppdaterat `Program.cs` att använda `UseNpgsql()`
5. ? Skapat nya migrationer för PostgreSQL

---

## ?? **Steg för att slutföra migrationen:**

### **1?? Hämta din Supabase Connection String**

1. Gå till https://supabase.com/dashboard
2. Välj ditt projekt "JobApplicationTracker"
3. Gå till **Settings** ? **Database**
4. Scrolla ner till **Connection string**
5. Välj **URI** format
6. Kopiera connection stringen

**Den ser ut ungefär så här:**
```
postgresql://postgres:YOUR-PASSWORD@db.xxx.supabase.co:5432/postgres
```

---

### **2?? Konvertera till Npgsql format**

Supabase ger dig en PostgreSQL URI, men Npgsql föredrar följande format:

**Från Supabase:**
```
postgresql://postgres:YOUR-PASSWORD@db.xxx.supabase.co:5432/postgres
```

**Till Npgsql (använd detta):**
```
Host=db.xxx.supabase.co;Database=postgres;Username=postgres;Password=YOUR-PASSWORD;SSL Mode=Require;Trust Server Certificate=true
```

**Ersätt:**
- `db.xxx.supabase.co` med din riktiga Supabase host
- `YOUR-PASSWORD` med ditt faktiska databas-lösenord

---

### **3?? Uppdatera appsettings.json**

Öppna `appsettings.json` och uppdatera connection stringen:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=db.xxx.supabase.co;Database=postgres;Username=postgres;Password=YOUR-PASSWORD;SSL Mode=Require;Trust Server Certificate=true"
  }
}
```

?? **VIKTIGT: Använd User Secrets för känslig data!**

---

### **4?? (REKOMMENDERAT) Använd User Secrets istället**

För att inte committa lösenord till Git, använd User Secrets:

```powershell
# Sätt connection string som User Secret
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=db.xxx.supabase.co;Database=postgres;Username=postgres;Password=YOUR-PASSWORD;SSL Mode=Require;Trust Server Certificate=true"
```

Då kan du låta `appsettings.json` ha en placeholder:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=postgres;Username=postgres;Password=dummy"
  }
}
```

User Secrets kommer automatiskt att overrida denna i development!

---

### **5?? Applicera migrationer till Supabase**

```powershell
# Applicera migrationer
dotnet ef database update

# Om du får fel, kontrollera att:
# 1. Connection stringen är korrekt
# 2. Lösenordet är rätt
# 3. SSL Mode är "Require"
```

---

### **6?? Verifiera i Supabase Dashboard**

1. Gå till **Table Editor** i Supabase Dashboard
2. Du borde nu se följande tabeller:
   - `AspNetUsers`
   - `AspNetRoles`
   - `AspNetUserRoles`
   - `AspNetUserClaims`
   - `AspNetUserLogins`
   - `AspNetUserTokens`
   - `AspNetRoleClaims`
   - `JobApplications`
   - `__EFMigrationsHistory`

---

### **7?? Kör applikationen**

```powershell
dotnet run
```

Gå till `https://localhost:5001` och testa:
1. Registrera ett konto
2. Logga in
3. Skapa en job application
4. Verifiera i Supabase Dashboard att data sparas!

---

## ?? **Säkerhet: Hantera Connection Strings**

### **För Development (din dator):**
```powershell
# Använd User Secrets
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=db.xxx.supabase.co;Database=postgres;Username=postgres;Password=YOUR-PASSWORD;SSL Mode=Require;Trust Server Certificate=true"
```

### **För Production (Azure, Railway, etc.):**
Sätt som **Environment Variable**:
```
ConnectionStrings__DefaultConnection=Host=db.xxx.supabase.co;Database=postgres;Username=postgres;Password=YOUR-PASSWORD;SSL Mode=Require;Trust Server Certificate=true
```

### **Lägg till i `.gitignore`:**
```
# User Secrets
secrets.json

# Production config
appsettings.Production.json
```

---

## ?? **Exempel på fullständig connection string:**

**För ett projekt med ID `abcdefghij` och lösenord `MySecurePass123!`:**

```
Host=db.abcdefghij.supabase.co;Database=postgres;Username=postgres;Password=MySecurePass123!;SSL Mode=Require;Trust Server Certificate=true
```

---

## ?? **Felsökning**

### **Problem: "Connection refused"**
- ? Kontrollera att Supabase-projektet är aktivt (gå till Dashboard)
- ? Verifiera att host-namnet är korrekt (ska vara `db.xxx.supabase.co`)

### **Problem: "Password authentication failed"**
- ? Kontrollera lösenordet (hämta nytt från Settings ? Database ? Reset Database Password)
- ? Se till att det inte finns extra mellanslag i connection stringen

### **Problem: "SSL connection required"**
- ? Lägg till `SSL Mode=Require;Trust Server Certificate=true` i connection stringen

### **Problem: "Npgsql.PostgresException: 42P07: relation already exists"**
- ? Tabellerna finns redan - du kan ignorera detta eller köra:
  ```powershell
  dotnet ef database drop
  dotnet ef database update
  ```

---

## ?? **Fördelar med Supabase över SQLite:**

| Feature | SQLite | Supabase |
|---------|--------|----------|
| **Tillgänglig från alla datorer** | ? NEJ | ? JA |
| **Gratis** | ? JA | ? JA (500MB) |
| **Realtidssynk** | ? NEJ | ? JA |
| **Backup automatiskt** | ? NEJ | ? JA |
| **Kan skalas upp** | ? NEJ | ? JA |
| **Webbgränssnitt** | ? NEJ | ? JA |
| **REST API automatiskt** | ? NEJ | ? JA |

---

## ?? **Nästa steg:**

1. Sätt connection string (User Secrets eller appsettings.json)
2. Kör `dotnet ef database update`
3. Kör `dotnet run`
4. Testa appen!
5. Verifiera i Supabase Dashboard att data sparas

---

## ?? **Kommit-meddelande förslag:**

```
git add .
git commit -m "Migrate from SQLite to Supabase (PostgreSQL)

- Replaced Microsoft.EntityFrameworkCore.Sqlite with Npgsql.EntityFrameworkCore.PostgreSQL
- Updated Program.cs to use UseNpgsql() instead of UseSqlite()
- Added retry logic and connection timeout configuration
- Created new migrations for PostgreSQL
- Updated EF Core packages to v9.0.0
- Added SUPABASE_SETUP.md with detailed migration guide

Database is now cloud-based and accessible from any device!
"
```

---

Lycka till med din Supabase-migration! ??
