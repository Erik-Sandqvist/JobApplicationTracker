# Lösning: DbContext Scope-fel i Blazor Server

## Problemet

```
InvalidOperationException: Cannot resolve scoped service 
'System.Collections.Generic.IEnumerable`1[Microsoft.EntityFrameworkCore.Infrastructure.IDbContextOptionsConfiguration`1[JobApplicationTrackerV2.Data.ApplicationDbContext]]' 
from root provider.
```

## Orsak

Felet uppstod på grund av **dubbel registrering** av DbContext i `Program.cs`:

```csharp
// ? PROBLEM: Både Factory OCH vanlig DbContext registrerad samtidigt
builder.Services.AddDbContextFactory<ApplicationDbContext>(...);
builder.Services.AddDbContext<ApplicationDbContext>(..., ServiceLifetime.Scoped);
```

Detta skapade en konflikt där:
1. Entity Framework försökte resolve en `Scoped` DbContext från en root provider
2. Blazor Server försökte använda DbContext från fel scope

## Lösningen

Använd **DbContextPool** istället för vanlig DbContext för Identity-tjänster:

```csharp
// ? LÖSNING: Factory för Blazor + Pool för Identity
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
       maxRetryCount: 5,
          maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    }));

// Pool är mer effektiv än Scoped för Identity
builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
sqlOptions.EnableRetryOnFailure(
          maxRetryCount: 5,
       maxRetryDelay: TimeSpan.FromSeconds(30),
        errorNumbersToAdd: null);
    }));
```

## Varför detta fungerar

### DbContextFactory (för Blazor-komponenter)
- **Använd i**: `@inject IDbContextFactory<ApplicationDbContext>`
- **Livstid**: Transient (skapas vid behov)
- **Fördelar**:
  - Undviker concurrency-problem i Blazor Server
  - Explicit `using` statement kontrollerar livstiden
  - Perfekt för long-running circuits

### DbContextPool (för Identity/ASP.NET Core)
- **Använd av**: SignInManager, UserManager, etc.
- **Livstid**: Pooled (återanvänds mellan requests)
- **Fördelar**:
  - Bättre prestanda än Scoped
  - Automatisk context reset mellan requests
  - Fungerar seamless med Identity

## Best Practices för Blazor Server + EF Core

### ? GÖR:
```csharp
// I Blazor-komponenter
@inject IDbContextFactory<ApplicationDbContext> DbContextFactory

protected override async Task OnInitializedAsync()
{
    using var context = await DbContextFactory.CreateDbContextAsync();
    data = await context.MyEntities.ToListAsync();
}
```

### ? GÖR INTE:
```csharp
// Injicera ALDRIG DbContext direkt i Blazor Server-komponenter
@inject ApplicationDbContext Context // ? Detta orsakar scope-problem!
```

## Verifiering

Bygget lyckas nu utan fel:
```bash
dotnet build
# Build successful
```

## Referenser

- [Microsoft Docs: DbContext Pooling](https://learn.microsoft.com/en-us/ef/core/performance/advanced-performance-topics#dbcontext-pooling)
- [Blazor Server with EF Core](https://learn.microsoft.com/en-us/aspnet/core/blazor/blazor-server-ef-core)
- [DbContextFactory Pattern](https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/#using-a-dbcontext-factory-eg-for-blazor)
