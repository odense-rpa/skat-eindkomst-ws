# Migration Guide: C# to Python

## Overview

This document provides detailed information about the migration from C#/.NET Framework 4.7.2 to Python 3.8+.

## Migration Strategy

### Phase 1: Core Library (COMPLETED)
- ? Service configuration (`ServiceConfig.cs` ? `config.py`)
- ? Main client class (`EIndkomst.cs` ? `client.py`)
- ? Response parser (`eIndkomstParser.cs` ? `parsers/eindkomst_parser.py`)
- ? Utility classes (`Dates.cs` ? `utils/dates.py`, logging utilities)
- ? Console application (`Program.cs` ? `main.py`)

### Phase 2: Testing (PENDING)
- ? Unit tests migration (`DatesTests.cs` ? Python pytest)
- ? Integration tests with mock SOAP service
- ? End-to-end tests with demo environment

### Phase 3: Production Support (PLANNED)
- ? Production environment configuration
- ? Production certificate handling
- ? Error handling improvements
- ? Performance optimization

## Key Migration Decisions

### 1. SOAP Client Library

**C# Approach:**
- Uses WCF (Windows Communication Foundation)
- Proxy classes generated with `svcutil.exe`
- Static typing with generated classes

**Python Approach:**
- Uses Zeep library (pure Python SOAP client)
- Dynamic WSDL parsing
- More flexible but requires runtime validation

**Why Zeep?**
- Most mature Python SOAP library
- Good WS-Security support
- Active maintenance
- Cross-platform compatibility

### 2. Data Structures

**C# Approach:**
```csharp
DataTable IndkomstOplysningPersonHent(...)
```

**Python Approach:**
```python
pd.DataFrame indkomst_oplysning_person_hent(...)
```

**Why pandas?**
- Industry standard for tabular data in Python
- Rich data manipulation capabilities
- Easy export to CSV, Excel, databases
- Better than Python lists/dicts for large datasets

### 3. Certificate Handling

**C# Approach:**
- Windows Certificate Store only
- Uses `X509Certificate2` class
- Integrated with Windows security

**Python Approach:**
- Supports both Windows Certificate Store and PEM files
- Uses `cryptography` library
- Cross-platform compatible

**Implementation Notes:**
- Windows: Can use `wincertstore` package (future enhancement)
- Linux/Mac: PEM files recommended
- Docker: Mount certificates as secrets

### 4. Configuration Management

**C# Approach:**
- Uses `DotNetEnv` NuGet package
- Reads from `.env` file

**Python Approach:**
- Uses `python-dotenv` package
- Same `.env` file format
- Compatible configuration

### 5. Logging

**C# Approach:**
```csharp
public interface ILog
{
    void Info(string message);
    void Error(string message);
}
```

**Python Approach:**
```python
class ILog:
    def Info(self, message: str): ...
    def Error(self, message: str): ...

    # Python-style aliases
    def info(self, message: str): ...
    def error(self, message: str): ...
```

**Design Decision:**
- Maintain C# interface for compatibility
- Add Python-style methods (lowercase) as aliases
- Use Python's standard `logging` module internally

## Component Mapping

### Main Classes

| C# Class | Python Module | Notes |
|----------|---------------|-------|
| `EIndkomst` | `client.EIndkomst` | Main client class |
| `ServiceConfig` | `config.ServiceConfig` | Configuration dataclass |
| `eIndkomstParser` | `parsers.EIndkomstParser` | Response parser |
| `ILog` | `utils.logger.ILog` | Logging interface |
| `Dates` (utilities) | `utils.dates` | Date helper functions |

### Methods

| C# Method | Python Method | Return Type Change |
|-----------|---------------|-------------------|
| `IndkomstOplysningPersonHent()` | `indkomst_oplysning_person_hent()` | `DataTable` ? `DataFrame` |
| `EIndkomstPersonHentKlient()` | `eindkomst_person_hent_klient()` | `IndkomstOplysningPersonHent_OType` ? `Dict` |
| `ParseResultToDataTable()` | `parse_result_to_dataframe()` | `DataTable` ? `DataFrame` |

### Naming Conventions

**C# Conventions:**
- PascalCase for classes and methods
- camelCase for parameters
- Private fields prefixed with underscore

**Python Conventions:**
- PascalCase for classes
- snake_case for functions and methods
- snake_case for parameters
- Private methods/attributes prefixed with underscore

**Migration Strategy:**
- Public API methods maintain C#-style naming where it makes sense
- Internal methods use Python conventions
- Parameters converted to snake_case

## SOAP/WCF Migration Details

### Auto-Generated Proxy Classes

**C# Approach:**
```bash
svcutil https://eksternwiki.skat.dk/services/demotin/eIndkomst10/IndkomstOplysningPersonHent.wsdl /out:EIndkomstProxy.cs /namespace:*,dk.skat.eindkomst /syncOnly /noConfig
```

Generates:
- `EIndkomstProxy.cs` (Demo)
- `EIndkomstProxyProd.cs` (Prod)
- ~2000 lines of code per file
- Static types for all SOAP objects

**Python Approach:**
```python
from zeep import Client

# Using local WSDL file (included in project)
client = Client('wsdl/eIndkomst10/IndkomstOplysningPersonHent.wsdl')

# Or using GitHub URL (fallback)
client = Client('https://raw.githubusercontent.com/skat/eksternwiki/main/services/demotin/eIndkomst10/IndkomstOplysningPersonHent.wsdl')
```

- No code generation needed
- Dynamic type creation at runtime
- WSDL parsed automatically
- Much smaller codebase
- Local WSDL files for faster initialization

**WSDL Management:**
- C#: Downloads WSDL, generates code, commits generated files
- Python: Includes WSDL files directly, no generation step needed

**Trade-offs:**
- C#: Compile-time type safety, IntelliSense
- Python: More flexible, less boilerplate, runtime validation, faster development

### WS-Security Implementation

**C# Approach:**
```csharp
// Uses System.ServiceModel.Security
// WCF handles WS-Security automatically
var binding = new CustomBinding();
binding.Elements.Add(new SecurityBindingElement());
```

**Python Approach:**
```python
from zeep.wsse.signature import Signature

wsse = Signature(
    key_file=signing_cert_path,
    cert_file=auth_cert_path,
    password=cert_password
)
client = Client(wsdl, wsse=wsse)
```

**Status:**
- ?? WS-Security implementation needs verification with real certificates
- ?? May require additional configuration for SKAT's specific requirements

## Data Type Conversions

### Simple Types

| C# Type | Python Type | Notes |
|---------|-------------|-------|
| `string` | `str` | Direct mapping |
| `int` | `int` | Direct mapping |
| `bool` | `bool` | Direct mapping |
| `DateTime` | `datetime.datetime` | Use `datetime` module |
| `decimal` | `float` or `Decimal` | Use `Decimal` for financial data |

### Collection Types

| C# Type | Python Type | Notes |
|---------|-------------|-------|
| `List<T>` | `list[T]` | Direct mapping |
| `Dictionary<K,V>` | `dict[K,V]` | Direct mapping |
| `DataTable` | `pd.DataFrame` | Pandas library |

### Example: Date Handling

**C#:**
```csharp
DateTime.Parse("2023.01.01")
date.ToString("yyyy-MM-dd")
date.ToShortDateString()
```

**Python:**
```python
datetime.strptime("2023.01.01", "%Y.%m.%d")
date.strftime("%Y-%m-%d")
date.strftime("%d-%m-%Y")  # to_short_date_string()
```

## Error Handling

### C# Approach

```csharp
try
{
    var result = service.IndkomstOplysningPersonHent(...);
    return result;
}
catch (FaultException<IndkomstOplysningPersonHent_OType> ex)
{
    log.Error(ex.Message);
    throw;
}
catch (Exception ex)
{
    log.Error(ex.Message);
    throw;
}
```

### Python Approach

```python
try:
    result = service.IndkomstOplysningPersonHent(**request)
    return result
except Fault as e:
    self._log_error(f"SOAP Fault: {e}")
    raise
except Exception as e:
    self._log_error(f"Error: {e}")
    raise
```

**Improvements:**
- More specific exception types in Python
- Better error messages with f-strings
- Automatic traceback information

## Testing Strategy

### Unit Tests

**C# Tests:**
```csharp
[TestClass]
public class DatesTests
{
    [TestMethod]
    public void TestDateParsing()
    {
        // Test implementation
    }
}
```

**Python Tests (pytest):**
```python
class TestDates:
    def test_date_parsing(self):
        # Test implementation
        pass
```

**Migration Plan:**
1. Port existing C# unit tests to pytest
2. Add tests for new Python-specific functionality
3. Add integration tests with mock SOAP service
4. Add end-to-end tests with demo environment

### Test Coverage

**Planned Test Structure:**
```
tests/
??? unit/
?   ??? test_config.py
?   ??? test_client.py
?   ??? test_parser.py
?   ??? test_dates.py
??? integration/
?   ??? test_soap_client.py
??? e2e/
    ??? test_demo_environment.py
```

## Performance Considerations

### C#
- Compiled language
- Static typing
- JIT compilation
- Excellent performance

### Python
- Interpreted language
- Dynamic typing
- GIL (Global Interpreter Lock)
- Slower for pure compute

### Optimization Strategy
1. Use pandas for data processing (C-optimized)
2. Use zeep (uses lxml, which is C-based)
3. Consider async/await for concurrent requests
4. Profile and optimize hot paths
5. Consider Cython for critical sections (if needed)

## Deployment Differences

### C#
```
Build ? .dll assembly ? Copy to Blue Prism
```

### Python
```
Package ? Install with pip ? Run as script or import as library
```

**Python Deployment Options:**
1. **Virtual environment** (development)
2. **pip install** (system-wide)
3. **Docker container** (production)
4. **PyInstaller** (standalone executable)

## Known Limitations

### 1. Certificate Store Access
- **Issue:** Python doesn't have native Windows Certificate Store access
- **Workaround:** Use PEM files or implement `wincertstore` package
- **Status:** Pending enhancement

### 2. WSDL Complexity
- **Issue:** SKAT's WSDL is complex with nested structures
- **Impact:** May require manual adjustments to request structure
- **Status:** Needs testing with real service

### 3. Type Safety
- **Issue:** Python is dynamically typed
- **Mitigation:** Use type hints and mypy for static analysis
- **Status:** Type hints added, mypy validation pending

### 4. Blue Prism Integration
- **Issue:** Original C# version was a Blue Prism DLL
- **Resolution:** Not applicable - this is a standalone migration
- **Status:** N/A - by design

## Next Steps

### Immediate (Week 1-2)
1. ? Complete core library migration
2. ? Create demo console application
3. ? Test with demo environment
4. ? Validate SOAP requests/responses

### Short-term (Week 3-4)
1. ? Port unit tests
2. ? Add integration tests
3. ? Improve certificate handling
4. ? Add comprehensive error handling

### Long-term (Month 2+)
1. ? Production environment support
2. ? Performance optimization
3. ? Docker containerization
4. ? CI/CD pipeline
5. ? Documentation improvements

## Resources

### SKAT Documentation
- eIndkomst Wiki: https://eksternwiki.skat.dk
- WSDL Demo: https://eksternwiki.skat.dk/services/demotin/eIndkomst10/
- WSDL Prod: https://eksternwiki.skat.dk/services/prodtin/eIndkomst10/

### Python Libraries
- Zeep: https://docs.python-zeep.org/
- pandas: https://pandas.pydata.org/docs/
- cryptography: https://cryptography.io/
- python-dotenv: https://github.com/theskumar/python-dotenv

### Original C# Project
- Repository: https://github.com/odense-rpa/skat-eindkomst-ws
- License: MIT
- Authors: Odense Kommune

## Questions & Answers

### Q: Why not use suds-jurko instead of zeep?
**A:** Zeep is more actively maintained, has better WS-Security support, and works with Python 3.8+. suds-jurko is deprecated.

### Q: Can we run this in Blue Prism?
**A:** Not directly. Blue Prism uses .NET assemblies. However, you could:
- Call Python script from Blue Prism using "Utility - Environment"
- Use Python via REST API
- Use IronPython (limited Python 2.7)

### Q: How do we handle certificate renewal?
**A:** Same as C# version - update certificate files or Windows Certificate Store. No code changes needed if using paths in `.env`.

### Q: Is this production-ready?
**A:** Not yet. This is an experimental migration for demo environment only. Production use requires:
- Thorough testing
- Production certificate configuration
- Error handling improvements
- Performance validation

### Q: Can we use this on Linux/Mac?
**A:** Yes! That's one advantage of the Python migration. Use PEM certificate files instead of Windows Certificate Store.

## Conclusion

This migration demonstrates the feasibility of converting the SKAT eIndkomst C# client to Python. The core functionality has been successfully migrated, maintaining compatibility with the original design while leveraging Python's strengths.

The next phase focuses on testing and validation to ensure the Python client works correctly with the SKAT demo environment.
