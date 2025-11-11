# ?? Setup User Secrets för Supabase Connection String
# Detta script hjälper dig att sätta upp User Secrets så att du inte behöver committa lösenord till Git

Write-Host "?? Supabase User Secrets Setup" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

# Check if project has user secrets initialized
$hasUserSecrets = dotnet user-secrets list 2>&1

if ($LASTEXITCODE -ne 0) {
    Write-Host "?? Initierar User Secrets..." -ForegroundColor Yellow
    dotnet user-secrets init
    Write-Host ""
}

Write-Host "??  Du behöver följande information från Supabase:" -ForegroundColor Yellow
Write-Host "   1. Gå till https://supabase.com/dashboard" -ForegroundColor Gray
Write-Host "   2. Välj ditt projekt" -ForegroundColor Gray
Write-Host "   3. Gå till Settings ? Database" -ForegroundColor Gray
Write-Host "   4. Hitta 'Connection string' (URI format)" -ForegroundColor Gray
Write-Host ""

# Prompt for Supabase details
Write-Host "?? Ange dina Supabase-uppgifter:" -ForegroundColor Cyan
Write-Host ""

$host_input = Read-Host "Supabase Host (ex: db.abcdefghij.supabase.co)"
$password_input = Read-Host "Database Password" -AsSecureString
$password_plain = [Runtime.InteropServices.Marshal]::PtrToStringAuto(
    [Runtime.InteropServices.Marshal]::SecureStringToBSTR($password_input))

# Build connection string
$connectionString = "Host=$host_input;Database=postgres;Username=postgres;Password=$password_plain;SSL Mode=Require;Trust Server Certificate=true"

Write-Host ""
Write-Host "?? Sparar connection string i User Secrets..." -ForegroundColor Yellow

# Set the user secret
dotnet user-secrets set "ConnectionStrings:DefaultConnection" $connectionString

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "? User Secrets konfigurerade framgångsrikt!" -ForegroundColor Green
    Write-Host ""
    Write-Host "?? Connection string format:" -ForegroundColor Cyan
    Write-Host "   Host=$host_input" -ForegroundColor Gray
    Write-Host "   Database=postgres" -ForegroundColor Gray
    Write-Host "   Username=postgres" -ForegroundColor Gray
    Write-Host "   Password=***********" -ForegroundColor Gray
    Write-Host "   SSL Mode=Require" -ForegroundColor Gray
    Write-Host ""
    Write-Host "?? För att se alla user secrets:" -ForegroundColor Cyan
    Write-Host "   dotnet user-secrets list" -ForegroundColor Gray
    Write-Host ""
    Write-Host "???  För att ta bort user secrets:" -ForegroundColor Cyan
    Write-Host "   dotnet user-secrets clear" -ForegroundColor Gray
    Write-Host ""
    Write-Host "??  Nästa steg:" -ForegroundColor Cyan
    Write-Host "   1. Kör: dotnet ef database update" -ForegroundColor Yellow
 Write-Host "   2. Kör: dotnet run" -ForegroundColor Yellow
    Write-Host "   3. Testa appen!" -ForegroundColor Yellow
    Write-Host ""
} else {
    Write-Host ""
    Write-Host "? Kunde inte sätta user secrets!" -ForegroundColor Red
    Write-Host "   Kör detta manuellt:" -ForegroundColor Yellow
    Write-Host "   dotnet user-secrets set `"ConnectionStrings:DefaultConnection`" `"$connectionString`"" -ForegroundColor Gray
    Write-Host ""
}

# Clear sensitive data from memory
$password_plain = $null
$connectionString = $null
