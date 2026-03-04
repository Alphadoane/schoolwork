@echo off
setlocal enabledelayedexpansion

echo ============================================================
echo   ATM Management System (C#) - Launching...
echo ============================================================
echo.

:: Check for dotnet
where dotnet >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERROR] .NET SDK not found. Please install .NET to run this project.
    pause
    exit /b 1
)

:: Navigate to project directory
cd /d "%~dp0ass3\AtmSystem"

:: Start Web App in a new window
echo Starting ASP.NET Core server...
start "ATM System Server" cmd /c "dotnet run --urls=http://localhost:5000"

:: Wait for a few seconds for the server to start
echo Waiting for server to initialize...
timeout /t 8 /nobreak >nul

:: Open browser
echo Opening browser...
start http://localhost:5000

echo.
echo ============================================================
echo   Server is running in a separate window.
echo   You can access the system at http://localhost:5000
echo ============================================================
echo.
pause
