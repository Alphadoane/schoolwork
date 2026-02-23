@echo off
REM ============================================================
REM  SETUP SCRIPT – C# Assignment Projects (Questions 1, 2, 3)
REM  Downloads NuGet CLI and restores System.Data.SQLite package
REM ============================================================

echo.
echo  ============================================================
echo   C# Assignment Setup – Downloading NuGet and SQLite
echo  ============================================================
echo.

REM Download NuGet CLI if not present
if not exist nuget.exe (
    echo  [1/3] Downloading NuGet CLI...
    powershell -Command "Invoke-WebRequest -Uri 'https://dist.nuget.org/win-x86-commandline/latest/nuget.exe' -OutFile 'nuget.exe'"
    echo  Done.
) else (
    echo  [1/3] NuGet CLI already present.
)

echo.
echo  [2/3] Restoring packages for Question 1 (EVMS)...
nuget.exe restore Question1\Question1.csproj -PackagesDirectory Question1\packages
echo.

echo  [3/3] Restoring packages for Question 2 (Bonga Points)...
nuget.exe restore Question2\Question2.csproj -PackagesDirectory Question2\packages
echo.

echo  [4/4] Restoring packages for Question 3 (Fortune BS)...
nuget.exe restore Question3\Question3.csproj -PackagesDirectory Question3\packages
echo.

echo  ============================================================
echo   Setup complete! Open the .csproj files in Visual Studio
echo   or use MSBuild to compile each project.
echo  ============================================================
echo.
pause
