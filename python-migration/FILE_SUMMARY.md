# File Summary

## Project Structure

```
python-migration/
??? src/skat_eindkomst/           # Main package
?   ??? client.py                 # SOAP client
?   ??? config.py                 # ServiceConfig
?   ??? parsers/eindkomst_parser.py  # Response parser
?   ??? utils/
?       ??? logger.py             # ILog interface
?       ??? dates.py              # Date utilities
??? wsdl/eIndkomst10/             # Local WSDL files
??? main.py                       # Console demo
??? requirements.txt              # Dependencies
??? .env.example                  # Configuration template
??? [docs]                        # README, QUICKSTART, etc
```

## Key Files

| File | Purpose |
|------|---------|
| `main.py` | Demo console runner |
| `client.py` | SOAP client with WS-Security |
| `config.py` | Configuration from environment |
| `requirements.txt` | Python dependencies |
| `.env.example` | Configuration template |
| `wsdl/eIndkomst10/*.wsdl` | Local WSDL for offline support |

## Dependencies

- **zeep** - SOAP client
- **cryptography** - Certificate handling
- **python-dotenv** - Environment config
- **pandas** - Data processing
- **lxml** - XML handling
- **requests** - HTTP transport

## Documentation

| File | Purpose |
|------|---------|
| `README.md` | Main documentation |
| `QUICKSTART.md` | 5-minute setup |
| `MIGRATION_GUIDE.md` | C# ? Python mapping |
| `WSDL_GUIDE.md` | WSDL configuration |
| `CHANGELOG.md` | Version history |
| `STATUS.md` | Current operational status |

## Running

```bash
python main.py
```

Outputs:
- JSON response to console + `eindkomst_*.json`
- Raw SOAP XML: `soap_request_*.xml`, `soap_response_*.xml`
