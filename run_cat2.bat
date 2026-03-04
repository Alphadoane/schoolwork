@echo off
setlocal enabledelayedexpansion

echo ============================================================
echo   Student Result Slip System - Launching...
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
cd /d "%~dp0cat 2"

:: Start Django server in a new window
echo Starting Django server...
start "Result System Server" cmd /c "python manage.py runserver"

:: Wait for a few seconds for the server to start
echo Waiting for server to initialize...
timeout /t 5 /nobreak >nul

:: Open browser
echo Opening browser...
start http://127.0.0.1:8000

echo.
echo ============================================================
echo   Server is running in a separate window.
echo   You can access the system at http://127.0.0.1:8000
echo ============================================================
echo.
pause
