# Konvertera till SQLite - Kodändringar

## Steg 1: Installera SQLite-paket

```powershell
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

## Steg 2: Uppdatera Program.cs

### HITTA DESSA RADER (2 ställen i Program.cs):

```csharp
// RAD ~38
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
   maxRetryDelay: TimeSpan.FromSeconds(30),
        errorNumbersToAdd: null);
 }));

// RAD ~47
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
   sqlOptions.EnableRetryOnFailure(
       maxRetryCount: 5,
          maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    }));
```

### ÄNDRA TILL:

```csharp
// RAD ~38 - Ändra till UseSqlite
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// RAD ~47 - Ändra till UseSqlite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
```

## Komplett exempel på Program.cs med SQLite:

```csharp
using JobApplicationTrackerV2.Components.Account;
using JobApplicationTrackerV2.Components;
using JobApplicationTrackerV2.Data;
using JobApplicationTrackerV2.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Detailed errors for debugging
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddServerSideBlazor().AddCircuitOptions(options =>
    {
        options.DetailedErrors = true;
    });
}

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Data Source=jobapplications.db";  // Fallback till SQLite

// ? ÄNDRAT: UseSqlite istället för UseSqlServer
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// ? ÄNDRAT: UseSqlite istället för UseSqlServer
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
 .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

// Add MudBlazor
builder.Services.AddMudServices();

// Add ThemeService
builder.Services.AddScoped<ThemeService>();

var app = builder.Build();

// Automatically apply migrations in development with error handling
if (app.Environment.IsDevelopment())
{
    try
    {
        using (var scope = app.Services.CreateScope())
        {
          var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
      db.Database.Migrate();
   }
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
 // Continue running the app - migrations can be applied manually
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
 .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
```

## Steg 3: Uppdatera Connection String

### I User Secrets (Development):
```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Data Source=jobapplications.db"
```

### Eller i appsettings.json:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=jobapplications.db"
  }
}
```

## Steg 4: Skapa nya migrationer

```powershell
# Ta bort gamla SQL Server-migrationer
Remove-Item -Path "Data\Migrations\*" -Recurse -Force

# Skapa nya SQLite-migrationer
dotnet ef migrations add InitialCreate

# Skapa databasen
dotnet ef database update
```

## Steg 5: Testa!

```powershell
dotnet run
```

## ? Klart!

Din app använder nu SQLite - helt gratis! ??

**Databasfil:** `jobapplications.db`
**Kostnad:** 0 kr/månad
**Backup:** Kopiera .db-filen
