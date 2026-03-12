$env:PATH += ";$env:USERPROFILE\.dotnet"
Write-Host "============================================================" -ForegroundColor Green
Write-Host "  ATM Management System (C#) - Launching (PowerShell)..." -ForegroundColor Green
Write-Host "============================================================" -ForegroundColor Green
Write-Host ""

$root = Split-Path -Parent $MyInvocation.MyCommand.Definition
$projectPath = Join-Path $root "ass3\AtmSystem"

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host "[ERROR] .NET SDK not found. Please install .NET to run this project." -ForegroundColor Red
    Pause
    exit 1
}

Write-Host "Starting ASP.NET Core server in a new window..." -ForegroundColor Cyan
Start-Process powershell.exe -ArgumentList "-NoExit", "-Command", "Set-Location '$projectPath'; dotnet run --urls=http://localhost:5000"

Write-Host "Waiting for server to initialize..." -ForegroundColor Cyan
Start-Sleep -Seconds 8

# Launch DB Explorer after server initialization to ensure DB file is created
if (Test-Path $dbPath) { & "$root\Launch-DBExplorer.ps1" -DbPath $dbPath }

Write-Host "Opening browser..." -ForegroundColor Cyan
Start-Process "http://localhost:5000"

Write-Host ""
Write-Host "============================================================"
Write-Host "  Server is running in a separate PowerShell window."
Write-Host "  You can access the system at http://localhost:5000"
Write-Host "============================================================"
Write-Host ""
Pause

