@echo off
setlocal enabledelayedexpansion

:menu
cls
echo ============================================================
echo   C# Assignment 1 Projects - Launch Menu
echo ============================================================
echo.
echo  1. Question 1: Booker University Library System
echo  2. Question 2: Vehicle Sales Management System
echo  0. Exit
echo.
set /p choice="Enter your choice (1-2, 0 to exit): "

if "%choice%"=="1" (
    echo.
    echo Launching Question 1...
    cd /d "%~dp0assn1\Question1"
    dotnet run
    pause
    goto menu
)

if "%choice%"=="2" (
    echo.
    echo Launching Question 2...
    cd /d "%~dp0assn1\Question2"
    dotnet run
    pause
    goto menu
)

if "%choice%"=="0" exit

echo Invalid choice, please try again.
pause
goto menu
