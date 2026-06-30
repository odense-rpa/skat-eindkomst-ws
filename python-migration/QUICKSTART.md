# Quick Start Guide

## Get Started in 5 Minutes

### Prerequisites
- Python 3.8 or higher installed
- Access to the C# project's `.env` file (in parent directory)

### Step 1: Setup (Windows)

Open PowerShell or Command Prompt in the `python-migration` folder:

```cmd
cd C:\Users\bjras\source\repos\skat-eindkomst-ws\python-migration
```

Run the setup script:

```cmd
setup.bat
```

Or manually:

```cmd
# Create virtual environment
python -m venv venv

# Activate it
venv\Scripts\activate

# Install dependencies
pip install -r requirements.txt

# Copy environment file
copy .env.example .env
```

### Step 2: Configure

The setup script will copy `.env.example` to `.env`. 

**For demo testing**, the default values from the original C# project are already configured:

```ini
DEMO_SSN_1=2105440645
DEMO_SSN_2=2105440637
DEMO_USER=SystemNameTest
DEMO_DNS_IDENTITY=SKAT OIO Gateway Test
# ... etc
```

No changes needed for basic demo testing!

### Step 3: Run

With the virtual environment activated:

```cmd
python main.py
```

You should see output like:

```
Loading environment from: C:\Users\bjras\source\repos\skat-eindkomst-ws\.env

================================================================================
SKAT eIndkomst Python Console Application
Python Migration from C#/.NET Framework 4.7.2
================================================================================

INFO: SOAP client initialized for demo environment
================================================================================
Kalder eindkomst IndkomstOplysningPersonHent TEST
Bruger: SystemNameTest, ssn: 2105440645, ID: TEST XXXXXX 20240315_143022.456
Available test SSNs: 2105440645, 2105440637, 2105440629, 2105440599, 2105440521
================================================================================
INFO: Current date period: 2023-01-01 2023-12-30
INFO: Calling IndkomstOplysningPersonHent for SSN: 2105440645
...
```

## Common Commands

### Activate Virtual Environment

**Windows:**
```cmd
venv\Scripts\activate
```

**Linux/Mac:**
```bash
source venv/bin/activate
```

### Run Console Application

```cmd
python main.py
```

### Use as Library

Create a script `my_script.py`:

```python
from datetime import datetime
from skat_eindkomst import EIndkomst, ServiceConfig

# Load config from environment
config = ServiceConfig.from_env("demo")

# Create client
client = EIndkomst(config, environment="demo")

# Fetch data
df = client.indkomst_oplysning_person_hent(
    ssn="2105440645",
    worker_id="SystemNameTest",
    start_date=datetime(2023, 1, 1),
    end_date=datetime(2023, 12, 31),
    request_id="MY_REQUEST_001"
)

# Display results
print(df)
```

Run it:
```cmd
python my_script.py
```

### Inspect WSDL

The project includes local WSDL files for faster operation:

```cmd
# Inspect local WSDL
python -m zeep wsdl/eIndkomst10/IndkomstOplysningPersonHent.wsdl

# Or inspect from GitHub
python -m zeep https://raw.githubusercontent.com/skat/eksternwiki/main/services/demotin/eIndkomst10/IndkomstOplysningPersonHent.wsdl
```

This shows you the SOAP service structure, methods, and data types.

## Testing Different SSNs

Edit `.env` or set environment variable:

```cmd
set DEMO_SSN_1=2105440637
python main.py
```

Or modify `main.py` to loop through all SSNs:

```python
def run_demo():
    demo_ssns = get_demo_ssns()

    for ssn in demo_ssns:
        print(f"\n{'='*80}")
        print(f"Testing SSN: {ssn}")
        print('='*80)

        # ... rest of code
```

## Output Files

The console application saves results to CSV:

```
eindkomst_2105440645_20240315_143022.csv
```

Open in Excel, pandas, or any CSV reader.

## Troubleshooting

### "Python not found"
- Install Python 3.8+ from python.org
- Make sure Python is in your PATH

### "No module named 'zeep'"
- Activate virtual environment: `venv\Scripts\activate`
- Install dependencies: `pip install -r requirements.txt`

### Certificate errors
- For demo testing, certificate handling may need adjustment
- Check MIGRATION_GUIDE.md for certificate setup details

### SOAP errors
- Verify network connectivity to SKAT services
- Check that you're using valid demo SSNs
- Review logs for specific error messages

## Next Steps

1. **Review the output** - Check the CSV file and console output
2. **Read the docs** - See README.md and MIGRATION_GUIDE.md
3. **Explore the code** - Browse `src/skat_eindkomst/`
4. **Test different dates** - Modify date ranges in `main.py`
5. **Try different SSNs** - Use other demo SSNs from `.env`

## Getting Help

- Check `README.md` for detailed documentation
- Check `MIGRATION_GUIDE.md` for migration details
- Review the original C# code in parent directory
- Open an issue on GitHub

## Development Mode

To work on the library itself:

```cmd
# Install in editable mode
pip install -e .

# Now you can modify src/ files and test immediately
python main.py
```

## Comparing with C# Version

Run the C# version:
```cmd
cd ..\Odk.BluePrism.Skat.ConsoleApp\bin\Debug
Odk.BluePrism.Skat.ConsoleApp.exe
```

Run the Python version:
```cmd
cd C:\Users\bjras\source\repos\skat-eindkomst-ws\python-migration
python main.py
```

Compare outputs!

## Summary

You now have:
- ? Python environment set up
- ? Dependencies installed
- ? Configuration ready
- ? Demo application working
- ? Basic understanding of the migration

Happy coding! ??
