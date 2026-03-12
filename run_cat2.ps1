$env:PATH += ";$env:USERPROFILE\.dotnet"
Write-Host "============================================================" -ForegroundColor Blue
Write-Host "  Student Result Slip System (C#) - Launching (PowerShell)..." -ForegroundColor Blue
Write-Host "============================================================" -ForegroundColor Blue
Write-Host ""

$root = Split-Path -Parent $MyInvocation.MyCommand.Definition
$projectPath = Join-Path $root "cat 2\Cat2System"

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host "[ERROR] .NET SDK not found. Please install .NET to run this project." -ForegroundColor Red
    Pause
    exit 1
}

Write-Host "Starting ASP.NET Core server in a new window..." -ForegroundColor Cyan
Start-Process powershell.exe -ArgumentList "-NoExit", "-Command", "Set-Location '$projectPath'; dotnet run --urls=http://localhost:5005"

Write-Host "Waiting for server to initialize..." -ForegroundColor Cyan
Start-Sleep -Seconds 8

# Launch DB Explorer after server initialization
if (Test-Path $dbPath) { & "$root\Launch-DBExplorer.ps1" -DbPath $dbPath }
if (Test-Path $altDbPath) { & "$root\Launch-DBExplorer.ps1" -DbPath $altDbPath }

Write-Host "Opening browser..." -ForegroundColor Cyan
Start-Process "http://localhost:5005"

Write-Host ""
Write-Host "============================================================"
Write-Host "  Server is running in a separate PowerShell window."
Write-Host "  You can access the system at http://localhost:5005"
Write-Host "============================================================"
Write-Host ""
Pause

