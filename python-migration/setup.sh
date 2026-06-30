#!/bin/bash
# Setup script for SKAT eIndkomst Python migration

echo "=================================================="
echo "SKAT eIndkomst Python Migration - Setup"
echo "=================================================="
echo ""

# Check Python version
echo "Checking Python version..."
python --version

if [ $? -ne 0 ]; then
    echo "ERROR: Python is not installed or not in PATH"
    exit 1
fi

# Create virtual environment
echo ""
echo "Creating virtual environment..."
python -m venv venv

# Activate virtual environment
echo "Activating virtual environment..."
if [[ "$OSTYPE" == "msys" || "$OSTYPE" == "win32" ]]; then
    source venv/Scripts/activate
else
    source venv/bin/activate
fi

# Upgrade pip
echo ""
echo "Upgrading pip..."
pip install --upgrade pip

# Install dependencies
echo ""
echo "Installing dependencies..."
pip install -r requirements.txt

# Copy env example if .env doesn't exist
if [ ! -f .env ]; then
    echo ""
    echo "Creating .env file from .env.example..."
    cp .env.example .env
    echo "Please edit .env file with your configuration"
else
    echo ""
    echo ".env file already exists, skipping..."
fi

echo ""
echo "=================================================="
echo "Setup complete!"
echo "=================================================="
echo ""
echo "Next steps:"
echo "1. Activate the virtual environment:"
echo "   - Windows: venv\\Scripts\\activate"
echo "   - Linux/Mac: source venv/bin/activate"
echo ""
echo "2. Edit .env file with your configuration"
echo ""
echo "3. Run the demo application:"
echo "   python main.py"
echo ""
echo "=================================================="
