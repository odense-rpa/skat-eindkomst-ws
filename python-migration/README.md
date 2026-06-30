# SKAT eIndkomst Python Client

Python migration of the C#/.NET Framework 4.7.2 SKAT eIndkomst web service client.

## Overview

This is a Python 3.8+ implementation of the SKAT eIndkomst SOAP web service client, originally built in C# for Blue Prism RPA integration. This migration removes the Blue Prism dependency and provides a standalone Python library for accessing Danish Tax Authority's income information services.

**Original Project:** `Odk.BluePrism.Skat` (C#/.NET Framework 4.7.2)  
**Migration Status:** Demo environment only (for testing and experimentation)

## Features

- ? SOAP/WCF client using Zeep library
- ? Support for Demo environment
- ? X.509 certificate-based authentication
- ? WS-Security 1.0 support
- ? Data parsing to pandas DataFrame (equivalent to C# DataTable)
- ? Environment-based configuration
- ? Compatible logging interface (ILog pattern)
- ? Production environment support (planned)
- ? Unit tests (planned)

## Installation

### Prerequisites

- Python 3.8 or higher
- pip (Python package manager)

### Setup

1. **Clone the repository:**
   ```bash
   cd C:\Users\bjras\source\repos\skat-eindkomst-ws\python-migration
   ```

2. **Create a virtual environment (recommended):**
   ```bash
   python -m venv venv

   # On Windows
   venv\Scripts\activate

   # On Linux/Mac
   source venv/bin/activate
   ```

3. **Install dependencies:**
   ```bash
   pip install -r requirements.txt
   ```

4. **Configure environment:**
   ```bash
   # Copy example environment file
   copy .env.example .env

   # Edit .env with your configuration (if needed)
   ```

## Configuration

The client uses environment variables for configuration. Copy `.env.example` to `.env` and adjust values as needed.

### Demo Configuration (Included)

```ini
# Demo Test SSNs (CPR numbers for testing)
DEMO_SSN_1=2105440645
DEMO_SSN_2=2105440637
DEMO_SSN_3=2105440629
DEMO_SSN_4=2105440599
DEMO_SSN_5=2105440521

# Demo User
DEMO_USER=SystemNameTest

# Service Configuration
DEMO_DNS_IDENTITY=SKAT OIO Gateway Test
DEMO_AUTH_CERT_NAME=OIO Gateway Klient 3 Test
DEMO_SIGNING_CERT_NAME=SKAT OIO Gateway Test
DEMO_SE_NUMMER=19552101
DEMO_ABONNEMENT_TYPE_KODE=3153
DEMO_ABONNENT_TYPE_KODE=0750
DEMO_ADGANG_FORMAAL_TYPE_KODE=171
```

### Certificate Configuration

The client supports two modes for certificate handling:

1. **Windows Certificate Store** (default on Windows):
   - Set `DEMO_AUTH_CERT_NAME` and `DEMO_SIGNING_CERT_NAME`
   - Certificates must be installed in the certificate store

2. **PEM Certificate Files** (recommended for Linux/cross-platform):
   - Set `DEMO_AUTH_CERT_PATH` and `DEMO_SIGNING_CERT_PATH`
   - Optionally set password variables if certificates are encrypted

## Usage

### Console Application

Run the demo console application:

```bash
python main.py
```

This will:
1. Load configuration from `.env`
2. Connect to SKAT demo service
3. Fetch income data for the first demo SSN
4. Display results in console
5. Save results to CSV file

### As a Library

```python
from datetime import datetime
from skat_eindkomst import EIndkomst, ServiceConfig

# Load configuration
config = ServiceConfig.from_env("demo")

# Create client
client = EIndkomst(config, environment="demo")

# Fetch income data
df = client.indkomst_oplysning_person_hent(
    ssn="2105440645",
    worker_id="SystemNameTest",
    start_date=datetime(2023, 1, 1),
    end_date=datetime(2023, 12, 31),
    request_id="REQ_20240101_120000",
    get_basis_month=False
)

# Work with pandas DataFrame
print(df.head())
df.to_csv("output.csv", index=False)
```

## Project Structure

```
python-migration/
??? src/
?   ??? skat_eindkomst/
?       ??? __init__.py              # Package initialization
?       ??? client.py                # Main EIndkomst client (from EIndkomst.cs)
?       ??? config.py                # ServiceConfig (from ServiceConfig.cs)
?       ??? parsers/
?       ?   ??? __init__.py
?       ?   ??? eindkomst_parser.py  # Response parser (from eIndkomstParser.cs)
?       ??? utils/
?           ??? __init__.py
?           ??? dates.py             # Date utilities (from Dates.cs)
?           ??? logger.py            # Logging utilities (ILog interface)
??? wsdl/
?   ??? eIndkomst10/
?       ??? IndkomstOplysningPersonHent.wsdl  # Local WSDL file
?       ??? [XSD schema files...]             # Supporting schemas
??? main.py                          # Console app (from Program.cs)
??? requirements.txt                 # Python dependencies
??? pyproject.toml                   # Project metadata
??? .env.example                     # Example environment config
??? README.md                        # This file
```

## Migration Notes

### C# to Python Equivalents

| C# Component | Python Equivalent |
|--------------|-------------------|
| `System.Data.DataTable` | `pandas.DataFrame` |
| `System.ServiceModel` (WCF) | `zeep` library |
| `X509Certificate2` | `cryptography` + `requests` |
| `ILog` interface | Custom `ILog` class + Python `logging` |
| `DotNetEnv` | `python-dotenv` |
| `.csproj` / `.sln` | `pyproject.toml` |
| NuGet packages | `pip` / `requirements.txt` |

### Key Differences

1. **Certificate Handling:**
   - C# uses Windows Certificate Store exclusively
   - Python supports both certificate store and PEM files

2. **Data Structures:**
   - C# `DataTable` ? Python `pandas.DataFrame`
   - More flexible data manipulation in pandas

3. **SOAP Client:**
   - C# uses WCF with `svcutil.exe` generated proxies
   - Python uses Zeep library with dynamic WSDL parsing

4. **Error Handling:**
   - Python uses exceptions extensively
   - More Pythonic error handling patterns

## WSDL Information

The service uses TIN-enabled WSDL schemas (April 2023 version).

### Local WSDL Files

The project includes local WSDL files in the `wsdl/eIndkomst10/` folder for faster initialization and offline support:

```
python-migration/
??? wsdl/
    ??? eIndkomst10/
        ??? IndkomstOplysningPersonHent.wsdl
        ??? [schema files...]
```

The client automatically uses local WSDL files if available, providing:
- **Faster startup** - No network call needed
- **Offline support** - Works without internet connection
- **Version control** - WSDL is tracked in your repository
- **Reliability** - No dependency on external URLs

### Online WSDL Sources

If local WSDL files are not found, the client falls back to GitHub URLs:

- **Demo WSDL:** https://raw.githubusercontent.com/skat/eksternwiki/main/services/demotin/eIndkomst10/IndkomstOplysningPersonHent.wsdl
- **Prod WSDL:** https://raw.githubusercontent.com/skat/eksternwiki/main/services/prodtin/eIndkomst10/IndkomstOplysningPersonHent.wsdl

### Service Endpoints

The actual service endpoints (defined in WSDL):

- **Demo:** https://services.extranet.demo.skat.dk/vericert/services/IndkomstOplysningPersonHentV2ServicePort
- **Prod:** https://services.extranet.skat.dk/vericert/services/IndkomstOplysningPersonHentV2ServicePort

### Inspecting WSDL Structure

To inspect the WSDL structure using zeep:

```bash
# Using local file
python -m zeep wsdl/eIndkomst10/IndkomstOplysningPersonHent.wsdl

# Or using GitHub URL
python -m zeep https://raw.githubusercontent.com/skat/eksternwiki/main/services/demotin/eIndkomst10/IndkomstOplysningPersonHent.wsdl
```

## Certificate Setup

### Converting PFX to PEM (Windows)

If you have .pfx certificates from the C# project:

```bash
# Extract certificate
openssl pkcs12 -in cert.pfx -out cert.pem -nodes

# Or extract with password protection
openssl pkcs12 -in cert.pfx -out cert.pem -nodes -passin pass:your_password
```

### Linux/Mac Certificate Installation

Place certificate files in a secure location and update `.env`:

```ini
DEMO_AUTH_CERT_PATH=/path/to/auth-cert.pem
DEMO_SIGNING_CERT_PATH=/path/to/signing-cert.pem
```

## Development

### Installing in Development Mode

```bash
pip install -e .
```

### Code Style

This project follows PEP 8 style guidelines:

```bash
# Format code
black src/

# Check style
flake8 src/

# Type checking
mypy src/
```

## Troubleshooting

### Common Issues

1. **Certificate Errors:**
   - Verify certificates are valid and not expired
   - Check certificate paths in `.env`
   - Ensure proper permissions on certificate files

2. **SOAP Errors:**
   - Verify network connectivity to SKAT services
   - Check that demo SSNs are valid for the test environment
   - Review service configuration codes

3. **Import Errors:**
   - Ensure all dependencies are installed: `pip install -r requirements.txt`
   - Verify virtual environment is activated

## Future Work

- [ ] Complete production environment support
- [ ] Add comprehensive unit tests (port from C# tests)
- [ ] Add integration tests with mock SOAP service
- [ ] Improve certificate handling for Windows Certificate Store
- [ ] Add CLI interface with argparse
- [ ] Add async/await support for concurrent requests
- [ ] Docker container for easy deployment

## License

MIT License - Copyright (c) 2024 odense-rpa

## Original C# Project

This is a migration of the original C# project:
- **Repository:** https://github.com/odense-rpa/skat-eindkomst-ws
- **Original License:** MIT
- **Original Authors:** Odense Kommune

## Contributing

This is an experimental migration. Contributions are welcome, especially for:
- Production environment testing
- Certificate handling improvements
- Unit test coverage
- Documentation improvements

## Support

For issues related to the Python migration, please open an issue on GitHub.

For questions about the original SKAT eIndkomst service, refer to SKAT's official documentation.
