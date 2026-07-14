# WSDL Configuration

## Overview

Local WSDL files in `wsdl/eIndkomst10/` provide fast initialization and offline support.

## WSDL Location

```
wsdl/
??? eIndkomst10/
    ??? IndkomstOplysningPersonHent.wsdl  (Main WSDL)
    ??? [XSD schema files...]              (Schemas)
```

## How It Works

**Local WSDL (Primary):**
- Automatic detection if local files exist
- Fast startup, offline support, version controlled
- No external dependency

**GitHub Fallback:**
- Used if local files not found
- URLs: https://github.com/skat/eksternwiki
- Demo: `services/demotin/eIndkomst10/...`
- Prod: `services/prodtin/eIndkomst10/...`

## Service Endpoints

- **Demo:** https://services.extranet.demo.skat.dk/vericert/services/IndkomstOplysningPersonHentV2ServicePort
- **Prod:** https://services.extranet.skat.dk/vericert/services/IndkomstOplysningPersonHentV2ServicePort

## Using WSDL

```python
# Automatic (recommended)
from skat_eindkomst import EIndkomst, ServiceConfig

config = ServiceConfig.from_env("demo")
client = EIndkomst(config, environment="demo")  # Uses local WSDL automatically

# Manual
from zeep import Client
client = Client('wsdl/eIndkomst10/IndkomstOplysningPersonHent.wsdl')
```

## Inspecting WSDL

```bash
# Local WSDL
python -m zeep wsdl/eIndkomst10/IndkomstOplysningPersonHent.wsdl

# GitHub URL
python -m zeep https://raw.githubusercontent.com/skat/eksternwiki/main/services/demotin/eIndkomst10/IndkomstOplysningPersonHent.wsdl
```

Shows operations, request/response structures, and data types.

## Updating WSDL

Download from GitHub when SKAT updates:

```bash
cd wsdl/eIndkomst10
curl -O https://raw.githubusercontent.com/skat/eksternwiki/main/services/demotin/eIndkomst10/IndkomstOplysningPersonHent.wsdl

# Test
python main.py
```

## Troubleshooting

| Issue | Solution |
|-------|----------|
| WSDL not found | Check `wsdl/eIndkomst10/` exists; client falls back to GitHub |
| Schema errors | Ensure all XSD files present; download from GitHub |
| Network errors | Check internet; GitHub accessible |

## References

- GitHub: https://github.com/skat/eksternwiki
- Zeep: https://docs.python-zeep.org/
