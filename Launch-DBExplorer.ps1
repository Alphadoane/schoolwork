param (
    [string]$DbPath
)
$root = Split-Path -Parent $MyInvocation.MyCommand.Definition
$toolsDir = Join-Path $root "tools"
$zipPath = Join-Path $toolsDir "DB.Browser.for.SQLite-win64.zip"

$dbExeInfo = Get-ChildItem -Path $toolsDir -Filter "DB Browser for SQLite.exe" -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1
$dbExe = if ($dbExeInfo) { $dbExeInfo.FullName } else { $null }

if ([string]::IsNullOrWhiteSpace($dbExe) -or -not (Test-Path $dbExe)) {
    Write-Host "`nDownloading portable DB Browser for SQLite..." -ForegroundColor Cyan
    if (-not (Test-Path $toolsDir)) { New-Item -ItemType Directory -Path $toolsDir | Out-Null }
    
    # Download the portable zip
    try {
        Invoke-WebRequest -Uri "https://github.com/sqlitebrowser/sqlitebrowser/releases/download/v3.13.1/DB.Browser.for.SQLite-v3.13.1-win64.zip" -OutFile $zipPath -ErrorAction Stop
        Write-Host "Extracting DB Browser..." -ForegroundColor Cyan
        Expand-Archive -Path $zipPath -DestinationPath $toolsDir -Force
        
        $dbExeInfo = Get-ChildItem -Path $toolsDir -Filter "DB Browser for SQLite.exe" -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1
        $dbExe = if ($dbExeInfo) { $dbExeInfo.FullName } else { $null }
    } catch {
        Write-Host "[ERROR] Failed to download or extract DB Browser: $($_.Exception.Message)" -ForegroundColor Red
    }
}

if (-not [string]::IsNullOrWhiteSpace($dbExe) -and (Test-Path $dbExe)) {
    if (Test-Path $DbPath) {
        $psi = New-Object System.Diagnostics.ProcessStartInfo
        $psi.FileName = $dbExe
        $psi.Arguments = "`"$DbPath`""
        $psi.UseShellExecute = $false
        [System.Diagnostics.Process]::Start($psi) | Out-Null
    } else {
        Write-Host "[ERROR] Database not found at: $DbPath" -ForegroundColor Red
    }
} else {
    Write-Host "[WARNING] DB Browser executable not found, skipping database launch." -ForegroundColor Yellow
}
