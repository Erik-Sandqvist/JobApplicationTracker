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

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// ? FIX: Use ONLY DbContextFactory for Blazor Server
// This is the recommended approach to avoid concurrency issues
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString, sqlOptions =>
    {
        sqlOptions.CommandTimeout(30);
    }));

// ? Add pooled DbContext for Identity and other non-Blazor services
// This is more efficient than creating a new context every time
builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString, sqlOptions =>
    {
        sqlOptions.CommandTimeout(30);
    }));

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

// ? Automatically apply migrations in development with PROPER scoping
if (app.Environment.IsDevelopment())
{
    try
    {
        using var scope = app.Services.CreateScope();
  var services = scope.ServiceProvider;
     
        // Get DbContext from the scoped service provider
var context = services.GetRequiredService<ApplicationDbContext>();
      context.Database.Migrate();
    }
    catch (Exception ex)
    {
     var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database. Ensure SQL Server is running and accessible.");
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