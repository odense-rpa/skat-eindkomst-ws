@echo off
REM Setup script for SKAT eIndkomst Python migration (Windows)

echo ==================================================
echo SKAT eIndkomst Python Migration - Setup
echo ==================================================
echo.

REM Check Python version
echo Checking Python version...
python --version
if errorlevel 1 (
    echo ERROR: Python is not installed or not in PATH
    pause
    exit /b 1
)

REM Create virtual environment
echo.
echo Creating virtual environment...
python -m venv venv

REM Activate virtual environment
echo Activating virtual environment...
call venv\Scripts\activate.bat

REM Upgrade pip
echo.
echo Upgrading pip...
python -m pip install --upgrade pip

REM Install dependencies
echo.
echo Installing dependencies...
pip install -r requirements.txt

REM Copy env example if .env doesn't exist
if not exist .env (
    echo.
    echo Creating .env file from .env.example...
    copy .env.example .env
    echo Please edit .env file with your configuration
) else (
    echo.
    echo .env file already exists, skipping...
)

echo.
echo ==================================================
echo Setup complete!
echo ==================================================
echo.
echo Next steps:
echo 1. Activate the virtual environment:
echo    venv\Scripts\activate
echo.
echo 2. Edit .env file with your configuration
echo.
echo 3. Run the demo application:
echo    python main.py
echo.
echo ==================================================
echo.
pause
