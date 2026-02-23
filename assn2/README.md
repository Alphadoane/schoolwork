# C# Coursework - Advanced App Development

This repository contains solutions for three C# assignment questions, featuring class-based designs, interactive console drivers, and SQLite database persistence.

## Project Structure

- **Question 1 (EVMS)**: Electronic Voting Management System for adding, deleting, and displaying voter details.
- **Question 2 (Safaricom)**: Electronic Reward System for calculating and tracking Bonga points based on airtime.
- **Question 3 (Fortune BS)**: Employee Management System for tracking employee details and pension contributions.
- **setup.bat**: Script to download NuGet and restore required SQLite packages.
- **Assignment2.sln**: Visual Studio solution file for all three projects.

## Prerequisites

- Windows OS
- .NET Framework 4.5+ or Visual Studio
- Internet connection (for initial NuGet package restore)

## Setup and Installation

1. Open a terminal in the project root directory.
2. Run the `setup.bat` file:
   ```cmd
   setup.bat
   ```
   This script will download `nuget.exe` (if not present) and restore the `System.Data.SQLite` packages for all projects. It also ensures the native interoperability DLLs are available.

## Running the Applications

### From the Command Line
After running the setup, you can find the executables in the `bin\Debug` folder of each project:

- **Question 1**: `Question1\bin\Debug\Question1_EVMS.exe`
- **Question 2**: `Question2\bin\Debug\Question2_BongaPoints.exe`
- **Question 3**: `Question3\bin\Debug\Question3_FortuneBS.exe`

### From Visual Studio
Open `Assignment2.sln` in Visual Studio, build the solution, and run each project as needed.

## Database Information

Each application uses its own SQLite database file:
- **voters.db**: Stores voter records for Question 1.
- **subscribers.db**: Stores subscriber and Bonga point records for Question 2.
- **employees.db**: Stores employee records for Question 3.

The databases are automatically created upon the first run of the respective application.
