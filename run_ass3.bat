@echo off
setlocal enabledelayedexpansion

echo ============================================================
echo   ATM Management System - Launching...
echo ============================================================
echo.

:: Check for python
where python >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERROR] Python not found. Please install Python to run this project.
    pause
    exit /b 1
)

:: Navigate to project directory
cd /d "%~dp0ass3"

:: Start Flask app in a new window
echo Starting Flask server...
start "ATM System Server" cmd /c "python app.py"

:: Wait for a few seconds for the server to start
echo Waiting for server to initialize...
timeout /t 5 /nobreak >nul

:: Open browser
echo Opening browser...
start http://127.0.0.1:5000

echo.
echo ============================================================
echo   Server is running in a separate window.
echo   You can access the system at http://127.0.0.1:5000
echo ============================================================
echo.
pause
