# SKAT eIndkomst Python Client

Standalone Python port of the C#/.NET Framework 4.7.2 SKAT eIndkomst SOAP client (Blue Prism removed).

## Overview

Python 3.8+ implementation for accessing Danish Tax Authority's eIndkomst web service. Demo environment only.

**Original:** `Odk.BluePrism.Skat` (C#/.NET 4.7.2)  
**Status:** Demo-only, operational with JSON output and XML diagnostics

## Features

- SOAP/WCF client via Zeep
- X.509 certificate authentication (PEM files)
- WS-Security signing with BinarySecurityToken and Timestamp
- JSON response output
- Raw SOAP request/response XML capture for inspection
- Environment-based configuration (.env)
- ILog-compatible logging interface

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

Run demo:

```bash
python main.py
```

Outputs:
- JSON response to console and file
- Raw SOAP XML files for inspection (optional via `save_last_xml_exchange()`)

### As a Library

```python
from datetime import datetime
from skat_eindkomst import EIndkomst, ServiceConfig

# Create client
config = ServiceConfig(
    dns_identity="SKAT OIO Gateway Test",
    authentication_cert_path="path/to/cert.pem",
    signing_cert_path="path/to/key.pem",
    se_nummer="19552101",
    abonnement_type_kode="3153",
    abonnent_type_kode="0750",
    adgang_formaal_type_kode="171"
)
client = EIndkomst(config, environment="demo")

# Call service - returns JSON-compatible dict
result = client.indkomst_oplysning_person_hent(
    ssn="2105440645",
    worker_id="TestUser",
    start_date=datetime(2023, 1, 1),
    end_date=datetime(2023, 12, 31),
    request_id="TEST_001"
)

# Save raw SOAP XML for inspection
req_file, resp_file = client.save_last_xml_exchange()
print(f"Request saved to: {req_file}")
print(f"Response saved to: {resp_file}")
```

### Output Formats

The client returns JSON-compatible data from the SOAP service:

```python
import json

# Result is serialized to dict/JSON
print(json.dumps(result, indent=2))

# Save to file
with open("response.json", "w") as f:
    json.dump(result, f, indent=2, default=str)
```

### XML Diagnostics

To inspect raw SOAP request/response XML:

```python
# After any successful call
req_file, resp_file = client.save_last_xml_exchange()

# Files contain timestamped SOAP envelopes with WS-Security headers
# Format: soap_request_YYYYMMDD_HHMMSS.xml
#         soap_response_YYYYMMDD_HHMMSS.xml
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
