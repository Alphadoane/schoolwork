$env:PATH += ";$env:USERPROFILE\.dotnet"
function Show-Menu {
    Clear-Host
    Write-Host "============================================================" -ForegroundColor Cyan
    Write-Host "  C# Assignment 1 Projects - Launch Menu (PowerShell)" -ForegroundColor Cyan
    Write-Host "============================================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "  1. Question 1: Booker University Library System"
    Write-Host "  2. Question 2: Vehicle Sales Management System"
    Write-Host "  0. Exit"
    Write-Host ""
}

$root = Split-Path -Parent $MyInvocation.MyCommand.Definition

do {
    Show-Menu
    $choice = Read-Host "Enter your choice (1-2, 0 to exit)"

    switch ($choice) {
        "1" {
            Write-Host "`nLaunching Question 1..." -ForegroundColor Yellow
            $dbPath = Join-Path "$root\assn1\Question1" "books.db"
            if (Test-Path $dbPath) { & "$root\Launch-DBExplorer.ps1" -DbPath $dbPath }
            Set-Location -Path "$root\assn1\Question1"
            dotnet run
            Pause
        }
        "2" {
            Write-Host "`nLaunching Question 2..." -ForegroundColor Yellow
            $dbPath = Join-Path "$root\assn1\Question2" "vehicles.db"
            if (Test-Path $dbPath) { & "$root\Launch-DBExplorer.ps1" -DbPath $dbPath }
            Set-Location -Path "$root\assn1\Question2"
            dotnet run
            Pause
        }
        "0" {
            Write-Host "Goodbye!"
            break
        }
        default {
            Write-Host "Invalid choice, please try again." -ForegroundColor Red
            Pause
        }
    }
} while ($choice -ne "0")

