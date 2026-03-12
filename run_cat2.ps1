$env:PATH += ";$env:USERPROFILE\.dotnet"
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host "  Student Results System (C# Console) - Launching..." -ForegroundColor Cyan
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host ""

$root = Split-Path -Parent $MyInvocation.MyCommand.Definition
$projectPath = Join-Path $root "cat 2\Cat2System"

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host "[ERROR] .NET SDK not found. Please install .NET to run this project." -ForegroundColor Red
    Pause
    exit 1
}

Write-Host "Compiling and running console application..." -ForegroundColor Yellow
$dbPath = Join-Path $projectPath "bin\Debug\net10.0\results.db"

# Launch DB Explorer in background
Start-Job -ScriptBlock { param($r, $p) & "$r\Launch-DBExplorer.ps1" -DbPath $p } -ArgumentList $root, $dbPath | Out-Null

# Run the app
Set-Location $projectPath
dotnet run --project .
