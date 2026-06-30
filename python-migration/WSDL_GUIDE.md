# WSDL Configuration

## Overview

This project uses local WSDL files for the SKAT eIndkomst service, providing faster initialization and offline support.

## WSDL Location

```
python-migration/
??? wsdl/
    ??? eIndkomst10/
        ??? IndkomstOplysningPersonHent.wsdl  (Main WSDL file)
        ??? [XSD schema files...]              (Supporting XML schemas)
```

## How It Works

### Local WSDL (Primary)

The Python client automatically uses local WSDL files when available:

```python
# In client.py
DEMO_WSDL = "wsdl/eIndkomst10/IndkomstOplysningPersonHent.wsdl"
PROD_WSDL = "wsdl/eIndkomst10/IndkomstOplysningPersonHent.wsdl"  # Same file, different endpoint
```

**Benefits:**
- ? Faster startup (no network call)
- ? Works offline
- ? Version controlled
- ? No external dependency
- ? Consistent across environments

### GitHub Fallback (Secondary)

If local files are not found, the client falls back to GitHub URLs:

```python
DEMO_WSDL_GITHUB = "https://raw.githubusercontent.com/skat/eksternwiki/main/services/demotin/eIndkomst10/IndkomstOplysningPersonHent.wsdl"
PROD_WSDL_GITHUB = "https://raw.githubusercontent.com/skat/eksternwiki/main/services/prodtin/eIndkomst10/IndkomstOplysningPersonHent.wsdl"
```

## WSDL Sources

The WSDL files originate from SKAT's official repository:

- **GitHub:** https://github.com/skat/eksternwiki
- **Demo WSDL:** `services/demotin/eIndkomst10/IndkomstOplysningPersonHent.wsdl`
- **Prod WSDL:** `services/prodtin/eIndkomst10/IndkomstOplysningPersonHent.wsdl`

## Service Endpoints

The WSDL file includes the service endpoint definition, but these can also be configured programmatically:

### Demo Environment
```xml
<soap:address location="https://services.extranet.demo.skat.dk/vericert/services/IndkomstOplysningPersonHentV2ServicePort"/>
```

### Production Environment
```xml
<soap:address location="https://services.extranet.skat.dk/vericert/services/IndkomstOplysningPersonHentV2ServicePort"/>
```

## Using WSDL in Your Code

### Automatic (Recommended)

The `EIndkomst` client handles WSDL loading automatically:

```python
from skat_eindkomst import EIndkomst, ServiceConfig

config = ServiceConfig.from_env("demo")
client = EIndkomst(config, environment="demo")  # Automatically uses local WSDL
```

### Manual WSDL Path

If you need to specify a custom WSDL location:

```python
from zeep import Client

# Use local file
client = Client('wsdl/eIndkomst10/IndkomstOplysningPersonHent.wsdl')

# Use URL
client = Client('https://raw.githubusercontent.com/skat/eksternwiki/main/services/demotin/eIndkomst10/IndkomstOplysningPersonHent.wsdl')

# Use file system path
from pathlib import Path
wsdl_path = Path(__file__).parent / "wsdl" / "eIndkomst10" / "IndkomstOplysningPersonHent.wsdl"
client = Client(str(wsdl_path))
```

## Inspecting WSDL

### View Service Structure

Use zeep's command-line tool to inspect the WSDL:

```bash
# Local file
python -m zeep wsdl/eIndkomst10/IndkomstOplysningPersonHent.wsdl

# GitHub URL
python -m zeep https://raw.githubusercontent.com/skat/eksternwiki/main/services/demotin/eIndkomst10/IndkomstOplysningPersonHent.wsdl
```

Output shows:
- Available operations
- Request/response structures
- Data types
- Service endpoints

### Example Output

```
Global elements:
    ns0:IndkomstOplysningPersonHent_I
    ns0:IndkomstOplysningPersonHent_O

Global types:
    ...

Service: IndkomstOplysningPersonHentServiceBindingQSService
    Port: IndkomstOplysningPersonHentServiceBindingQSPort (Soap11Binding)
        Operations:
            getIndkomstOplysningPersonHent(Request: IndkomstOplysningPersonHent_I) -> Response: IndkomstOplysningPersonHent_O
```

## Updating WSDL Files

### When to Update

Update WSDL files when:
- SKAT releases a new service version
- Schema changes are announced
- New fields or operations are added

### How to Update

1. **Download from GitHub:**
   ```bash
   cd python-migration/wsdl/eIndkomst10

   # Download main WSDL
   curl -O https://raw.githubusercontent.com/skat/eksternwiki/main/services/demotin/eIndkomst10/IndkomstOplysningPersonHent.wsdl

   # Download supporting schemas (if needed)
   # Check the WSDL for xsd:include references
   ```

2. **Or clone the repository:**
   ```bash
   git clone https://github.com/skat/eksternwiki.git
   cp -r eksternwiki/services/demotin/eIndkomst10/* python-migration/wsdl/eIndkomst10/
   ```

3. **Verify the update:**
   ```bash
   python -m zeep wsdl/eIndkomst10/IndkomstOplysningPersonHent.wsdl
   ```

4. **Test with your application:**
   ```bash
   python main.py
   ```

## Troubleshooting

### WSDL Not Found

If you see errors about WSDL files not being found:

```
FileNotFoundError: [Errno 2] No such file or directory: 'wsdl/eIndkomst10/IndkomstOplysningPersonHent.wsdl'
```

**Solution:**
1. Check that the `wsdl/eIndkomst10` folder exists
2. Verify the WSDL file is present
3. The client will automatically fall back to GitHub URLs

### Schema Import Errors

If zeep reports schema import errors:

```
XMLSchemaParseError: ...
```

**Solution:**
1. Ensure all XSD files are present in the correct folder structure
2. Check that relative paths in the WSDL match your folder structure
3. Download missing schema files from GitHub

### Network Errors (GitHub Fallback)

If using GitHub fallback and seeing network errors:

```
ConnectionError: ...
```

**Solution:**
1. Check internet connectivity
2. Verify GitHub is accessible
3. Copy local WSDL files to avoid network dependency

## WSDL Version History

| Date | Version | Changes |
|------|---------|---------|
| April 2023 | TIN-enabled | Added Tax Identification Number (TIN) support |
| Current | eIndkomst10 | Latest stable version |

## Differences: Demo vs Production

Both environments use the **same WSDL file** structure, but with different:

- **Service endpoints** (demo.skat.dk vs skat.dk)
- **Certificates** (demo certs vs production certs)
- **Test data** vs real data

The WSDL itself is functionally identical between environments.

## Related Files

- `src/skat_eindkomst/client.py` - WSDL configuration
- `wsdl/eIndkomst10/IndkomstOplysningPersonHent.wsdl` - Main WSDL
- `wsdl/eIndkomst10/skat_dk/eindkomst/*.xsd` - Schema files

## References

- **SKAT eksternwiki GitHub:** https://github.com/skat/eksternwiki
- **SOAP/WSDL Specification:** https://www.w3.org/TR/wsdl/
- **Zeep Documentation:** https://docs.python-zeep.org/
