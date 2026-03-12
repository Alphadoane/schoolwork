$env:PATH += ";$env:USERPROFILE\.dotnet"
function Show-Menu {
    Clear-Host
    Write-Host "============================================================" -ForegroundColor Cyan
    Write-Host "  C# Assignment Projects - Launch Menu (PowerShell)" -ForegroundColor Cyan
    Write-Host "============================================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "  1. Question 1: Electronic Voting Management System (EVMS)"
    Write-Host "  2. Question 2: Safaricom Bonga Points System"
    Write-Host "  3. Question 3: Fortune Beauty Supplier (Fortune BS)"
    Write-Host "  0. Exit"
    Write-Host ""
}

$root = Split-Path -Parent $MyInvocation.MyCommand.Definition

do {
    Show-Menu
    $choice = Read-Host "Enter your choice (1-3, 0 to exit)"

    switch ($choice) {
        "1" {
            Write-Host "`nLaunching Question 1 (EVMS)..." -ForegroundColor Yellow
            $path = "$root\assn2\Question1\bin\Debug\Question1_EVMS.exe"
            if (Test-Path $path) {
                # Launch DB Explorer in background
                $dbPath = Join-Path $root "assn2\voters.db"
                Start-Job -ScriptBlock { param($r, $p) & "$r\Launch-DBExplorer.ps1" -DbPath $p } -ArgumentList $root, $dbPath | Out-Null
                Start-Process $path -Wait
            } else {
                Write-Host "Error: Executable not found at $path" -ForegroundColor Red
                Pause
            }
        }
        "2" {
            Write-Host "`nLaunching Question 2 (Bonga Points)..." -ForegroundColor Yellow
            $path = "$root\assn2\Question2\bin\Debug\Question2_BongaPoints.exe"
            if (Test-Path $path) {
                # Launch DB Explorer in background
                $dbPath = Join-Path $root "assn2\subscribers.db"
                Start-Job -ScriptBlock { param($r, $p) & "$r\Launch-DBExplorer.ps1" -DbPath $p } -ArgumentList $root, $dbPath | Out-Null
                Start-Process $path -Wait
            } else {
                Write-Host "Error: Executable not found at $path" -ForegroundColor Red
                Pause
            }
        }
        "3" {
            Write-Host "`nLaunching Question 3 (Fortune BS)..." -ForegroundColor Yellow
            $path = "$root\assn2\Question3\bin\Debug\Question3_FortuneBS.exe"
            if (Test-Path $path) {
                # Launch DB Explorer in background
                $dbPath = Join-Path $root "assn2\employees.db"
                Start-Job -ScriptBlock { param($r, $p) & "$r\Launch-DBExplorer.ps1" -DbPath $p } -ArgumentList $root, $dbPath | Out-Null
                Start-Process $path -Wait
            } else {
                Write-Host "Error: Executable not found at $path" -ForegroundColor Red
                Pause
            }
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

