# Quick Start Guide

## Get Started in 5 Minutes

### Prerequisites
- Python 3.8+ installed
- `.env` file configured (copy `.env.example` to `.env`)

### Step 1: Setup

```cmd
cd C:\Users\bjras\source\repos\skat-eindkomst-ws\python-migration

# Windows
python -m venv venv
venv\Scripts\activate
pip install -r requirements.txt

# Or Linux/Mac
python3 -m venv venv
source venv/bin/activate
pip install -r requirements.txt
```

### Step 2: Configure (Optional)

Copy `.env.example` to `.env`. Demo config is pre-configured with test SSNs:

```ini
DEMO_SSN_1=2105440645
DEMO_SSN_2=2105440637
DEMO_USER=SystemNameTest
DEMO_AUTH_CERT_PATH=path/to/cert.pem
DEMO_SIGNING_CERT_PATH=path/to/key.pem
```

### Step 3: Run

```cmd
python main.py
```

Outputs:
- JSON response to console
- `eindkomst_*.json` - Full JSON response
- `soap_request_*.xml` - Raw SOAP request
- `soap_response_*.xml` - Raw SOAP response

## Common Commands

```bash
# Activate environment
venv\Scripts\activate           # Windows
source venv/bin/activate       # Linux/Mac

# Run demo
python main.py

# Inspect local WSDL
python -m zeep wsdl/eIndkomst10/IndkomstOplysningPersonHent.wsdl

# Use as library
python my_script.py
```

## Use as Library

```python
from datetime import datetime
from skat_eindkomst import EIndkomst, ServiceConfig

config = ServiceConfig(
    dns_identity="SKAT OIO Gateway Test",
    authentication_cert_path="cert.pem",
    signing_cert_path="key.pem",
    se_nummer="19552101",
    abonnement_type_kode="3153",
    abonnent_type_kode="0750",
    adgang_formaal_type_kode="171"
)

client = EIndkomst(config, environment="demo")

# Returns JSON-compatible dict
result = client.indkomst_oplysning_person_hent(
    ssn="2105440645",
    worker_id="TestUser",
    start_date=datetime(2023, 1, 1),
    end_date=datetime(2023, 12, 31),
    request_id="TEST_001"
)

# Save raw SOAP XML
req_file, resp_file = client.save_last_xml_exchange()
print(f"Request: {req_file}\nResponse: {resp_file}")
```

## Output Formats

- **JSON:** Console output + `eindkomst_*.json` file
- **SOAP XML:** `soap_request_*.xml` and `soap_response_*.xml` for inspection

## Troubleshooting

| Issue | Fix |
|-------|-----|
| "No module named 'zeep'" | `pip install -r requirements.txt` |
| Certificate errors | Verify `.pem` files exist and paths in `.env` are correct |
| SOAP errors | Check network connectivity and demo SSN validity |
| Import errors | Ensure virtual environment is activated |

## Next Steps

1. Review JSON output and saved XML
2. Read `README.md` for detailed docs
3. Check `MIGRATION_GUIDE.md` for C# ? Python mapping
4. Explore code in `src/skat_eindkomst/`
- ? Demo application working
- ? Basic understanding of the migration

Happy coding! ??
