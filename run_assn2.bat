@echo off
setlocal enabledelayedexpansion

:menu
cls
echo ============================================================
echo   C# Assignment Projects - Launch Menu
echo ============================================================
echo.
echo  1. Question 1: Electronic Voting Management System (EVMS)
echo  2. Question 2: Safaricom Bonga Points System
echo  3. Question 3: Fortune Beauty Supplier (Fortune BS)
echo  0. Exit
echo.
set /p choice="Enter your choice (1-3, 0 to exit): "

if "%choice%"=="1" (
    echo.
    echo Launching Question 1 (EVMS)...
    cd /d "%~dp0assn2\Question1\bin\Debug"
    start Question1_EVMS.exe
    goto menu
)

if "%choice%"=="2" (
    echo.
    echo Launching Question 2 (Bonga Points)...
    cd /d "%~dp0assn2\Question2\bin\Debug"
    start Question2_BongaPoints.exe
    goto menu
)

if "%choice%"=="3" (
    echo.
    echo Launching Question 3 (Fortune BS)...
    cd /d "%~dp0assn2\Question3\bin\Debug"
    start Question3_FortuneBS.exe
    goto menu
)

if "%choice%"=="0" exit

echo Invalid choice, please try again.
pause
goto menu
