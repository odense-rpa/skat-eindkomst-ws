"""
Main client for SKAT eIndkomst web service
"""
from datetime import datetime
from typing import Optional, Dict, Any
from pathlib import Path
from zeep import Client, Settings
from zeep.wsse.signature import Signature
from zeep.helpers import serialize_object
from requests import Session
from zeep.transports import Transport

from .config import ServiceConfig
from .utils.logger import get_logger


class EIndkomst:
    """
    Python client for SKAT eIndkomst web service

    This class handles communication with the Danish Tax Authority's
    eIndkomst service using SOAP/WCF with certificate-based authentication.

    Migrated from C#/.NET Framework 4.7.2 to Python 3.8+
    Original: Odk.BluePrism.Skat (Blue Prism integration removed)
    """

    # WSDL file paths (local files in wsdl/eIndkomst10 folder)
    # Using local WSDL for faster initialization and offline support
    _WSDL_DIR = Path(__file__).parent.parent.parent / "wsdl" / "eIndkomst10"
    DEMO_WSDL = str(_WSDL_DIR / "IndkomstOplysningPersonHent.wsdl")
    PROD_WSDL = str(_WSDL_DIR / "IndkomstOplysningPersonHent.wsdl")  # Same WSDL, different endpoint

    # Alternative: GitHub URLs (if local WSDL not available)
    DEMO_WSDL_GITHUB = "https://raw.githubusercontent.com/skat/eksternwiki/main/services/demotin/eIndkomst10/IndkomstOplysningPersonHent.wsdl"
    PROD_WSDL_GITHUB = "https://raw.githubusercontent.com/skat/eksternwiki/main/services/prodtin/eIndkomst10/IndkomstOplysningPersonHent.wsdl"

    # Service endpoints (defined in WSDL but can be overridden)
    DEMO_ENDPOINT = "https://services.extranet.demo.skat.dk/vericert/services/IndkomstOplysningPersonHentV2ServicePort"
    PROD_ENDPOINT = "https://services.extranet.skat.dk/vericert/services/IndkomstOplysningPersonHentV2ServicePort"

    def __init__(self, config: ServiceConfig, environment: str = "demo", logger=None):
        """
        Initialize the eIndkomst client

        Args:
            config: Service configuration with certificates and codes
            environment: Either 'demo' or 'prod'
            logger: Optional logger instance (compatible with ILog interface)
        """
        self.config = config
        self.environment = environment.lower()
        self.logger = logger or get_logger(__name__)
        self.get_basis_month = False

        # Select WSDL and endpoint based on environment
        if self.environment == "prod":
            self.wsdl = self.PROD_WSDL
            self.endpoint = self.PROD_ENDPOINT
        else:
            self.wsdl = self.DEMO_WSDL
            self.endpoint = self.DEMO_ENDPOINT

        # Initialize SOAP client with security
        self._setup_client()

    def _log_info(self, message: str):
        """Log info message if logger is available"""
        if self.logger:
            if hasattr(self.logger, 'info'):
                self.logger.info(message)
            elif hasattr(self.logger, 'Info'):
                self.logger.Info(message)  # C# style

    def _log_error(self, message: str):
        """Log error message if logger is available"""
        if self.logger:
            if hasattr(self.logger, 'error'):
                self.logger.error(message)
            elif hasattr(self.logger, 'Error'):
                self.logger.Error(message)  # C# style

    def _setup_client(self):
        """
        Setup ZEEP SOAP client with WS-Security

        Uses local WSDL files from wsdl/eIndkomst10 folder for faster initialization.
        Falls back to GitHub URLs if local files are not available.
        """
        try:
            # Determine WSDL source
            wsdl_path = Path(self.wsdl)

            if wsdl_path.exists():
                self._log_info(f"Using local WSDL file: {wsdl_path}")
            else:
                # Fallback to GitHub URLs
                self._log_info(f"Local WSDL not found at {wsdl_path}")
                if self.environment == "prod":
                    self.wsdl = self.PROD_WSDL_GITHUB
                else:
                    self.wsdl = self.DEMO_WSDL_GITHUB
                self._log_info(f"Falling back to GitHub WSDL: {self.wsdl}")

            # Create session for transport
            session = Session()

            # Configure transport with session
            transport = Transport(session=session, timeout=60, operation_timeout=60)

            # Create ZEEP client with relaxed settings
            settings = Settings(
                strict=False,
                xml_huge_tree=True,
                xsd_ignore_sequence_order=True
            )

            # Configure WS-Security if certificates are provided
            # This must match the C# AbstractBindingFactory.CreateBinding() configuration:
            # - AsymmetricSecurityBindingElement with MutualCertificate
            # - WS-Security 1.0 (not 1.1)
            # - IncludeTimestamp = true
            # - SecurityHeaderLayout = Lax
            # - AllowSerializedSigningTokenOnReply = true
            wsse = None
            if self.config.authentication_cert_path and self.config.signing_cert_path:
                self._log_info(f"Loading certificates from:")
                self._log_info(f"  Auth: {self.config.authentication_cert_path}")
                self._log_info(f"  Sign: {self.config.signing_cert_path}")

                try:
                    from zeep.wsse.signature import BinarySignature
                    from zeep.wsse import utils

                    # Create BinarySignature (matches AsymmetricSecurityBindingElement)
                    # This creates BinarySecurityToken with X.509 certificate
                    signature = BinarySignature(
                        self.config.signing_cert_path,
                        self.config.authentication_cert_path,
                        self.config.authentication_cert_password or None
                    )

                    # Add Timestamp (IncludeTimestamp = true)
                    class BinarySignatureWithTimestamp(BinarySignature):
                        def apply(self, envelope, headers):
                            # Add timestamp first
                            security = utils.get_security_header(envelope)
                            timestamp = utils.WSU.Timestamp()
                            created = utils.WSU.Created()
                            expires = utils.WSU.Expires()

                            from datetime import datetime, timedelta
                            created.text = datetime.utcnow().strftime('%Y-%m-%dT%H:%M:%S.%fZ')
                            expires.text = (datetime.utcnow() + timedelta(minutes=5)).strftime('%Y-%m-%dT%H:%M:%S.%fZ')

                            timestamp.append(created)
                            timestamp.append(expires)
                            security.append(timestamp)

                            # Then apply signature
                            return super().apply(envelope, headers)

                        def verify(self, envelope):
                            # Disable response verification
                            pass

                    wsse = BinarySignatureWithTimestamp(
                        self.config.signing_cert_path,
                        self.config.authentication_cert_path,
                        self.config.authentication_cert_password or None
                    )

                    self._log_info("WS-Security configured: BinarySecurityToken + Timestamp (WS-Security 1.0)")

                except Exception as e:
                    self._log_error(f"Error configuring WS-Security: {e}")
                    self._log_info("This requires matching C# WCF AsymmetricSecurityBindingElement configuration")
                    raise

            self.client = Client(
                wsdl=self.wsdl,
                transport=transport,
                wsse=wsse,
                settings=settings
            )

            self._log_info(f"SOAP client initialized for {self.environment} environment")

            # Log the actual endpoint being used
            if hasattr(self.client.service, '_binding_options'):
                actual_endpoint = self.client.service._binding_options.get('address', 'unknown')
                self._log_info(f"Service endpoint: {actual_endpoint}")

        except Exception as e:
            self._log_error(f"Error setting up SOAP client: {str(e)}")
            raise

    def indkomst_oplysning_person_hent(
        self,
        ssn: str,
        worker_id: str,
        start_date: datetime,
        end_date: datetime,
        request_id: str,
        get_basis_month: bool = False
    ) -> Dict[str, Any]:
        """
        Retrieve income information for a person (IndkomstOplysningPersonHent)

        This is the main method that mirrors the C# method:
        DataTable IndkomstOplysningPersonHent(string ssn, string workerid, 
                                              DateTime startdate, DateTime enddate, 
                                              string id, bool getbasismonth = false)

        Args:
            ssn: Social security number (CPR number)
            worker_id: Worker/employee identifier (MedarbejderIdentifikator)
            start_date: Period start date
            end_date: Period end date
            request_id: Unique request identifier (TransaktionIdentifikator)
            get_basis_month: Whether to retrieve basis month data (HentBasisMaaned)

        Returns:
            Dictionary with JSON-serialized response data

        Raises:
            ValueError: If required parameters are missing
            Exception: If service call fails
        """
        self._log_info(
            f"Current date period: {start_date.strftime('%Y-%m-%d')} {end_date.strftime('%Y-%m-%d')}"
        )

        # Call the service and get raw response
        response = self.eindkomst_person_hent_klient(
            ssn, worker_id, start_date, end_date, request_id, get_basis_month
        )

        # Convert zeep response to JSON-serializable dict
        return serialize_object(response)

    def eindkomst_person_hent_klient(
        self,
        ssn: str,
        worker_id: str,
        start_date: datetime,
        end_date: datetime,
        request_id: str,
        get_basis_month: bool = False
    ) -> Dict[str, Any]:
        """
        Internal method to call the SOAP service

        This mirrors the C# method:
        IndkomstOplysningPersonHent_OType EIndkomstPersonHentKlient(...)

        Args:
            ssn: Social security number (CPR)
            worker_id: Worker ID
            start_date: Period start date
            end_date: Period end date
            request_id: Unique request identifier
            get_basis_month: Whether to retrieve basis month data

        Returns:
            Raw SOAP response as dictionary

        Raises:
            ValueError: If required parameters are missing or invalid
            Exception: If SOAP call fails
        """
        self.get_basis_month = get_basis_month

        # Validate parameters
        if not ssn or ssn.strip() == "":
            raise ValueError("'ssn' cannot be null or empty.")

        if not worker_id or worker_id.strip() == "":
            raise ValueError("'worker_id' cannot be null or empty.")

        if not request_id or request_id.strip() == "":
            raise ValueError("'request_id' cannot be null or empty.")

        if start_date >= end_date:
            raise ValueError("'start_date' must be before 'end_date'.")

        try:
            # Build SOAP request according to SKAT schema
            # Structure based on WSDL: HovedOplysninger + IndkomstOplysningPersonInddata

            self._log_info(f"Calling IndkomstOplysningPersonHent for SSN: {ssn}")
            self._log_info(f"Request ID: {request_id}")
            self._log_info(f"Worker ID: {worker_id}")
            self._log_info(f"Period: {start_date.strftime('%Y-%m-%d')} to {end_date.strftime('%Y-%m-%d')}")
            self._log_info(f"Get basis month: {get_basis_month}")

            # Create service binding
            service = self.client.service

            # Build request structure based on WSDL schema
            request_data = {
                'HovedOplysninger': {
                    'TransaktionIdentifikator': request_id,
                    'TransaktionTid': datetime.now()
                },
                'IndkomstOplysningPersonInddata': {
                    'AbonnentAdgangStruktur': {
                        'AbonnentTypeKode': int(self.config.abonnent_type_kode),
                        'AbonnementTypeKode': int(self.config.abonnement_type_kode),
                        'AdgangFormaalTypeKode': int(self.config.adgang_formaal_type_kode)
                    },
                    'AbonnentStruktur': {
                        'AbonnentVirksomhedStruktur': {
                            'AbonnentVirksomhed': {
                                'VirksomhedSENummerIdentifikator': self.config.se_nummer
                            }
                        },
                        'IndkomstOplysningAdgangMedarbejderIdentifikator': worker_id
                    },
                    'IndkomstOplysningValg': {
                        'IndkomstPersonSamling': {
                            'PersonIndkomstSoegeStruktur': [{
                                'PersonCivilRegistrationIdentifier': ssn,
                                'IndkomstOplysningAdgangReferenceNummerIdentifikator': request_id,
                                # Use date period or basis month
                                'SoegePeriodeLukketStruktur': {
                                    'DateInterval': {
                                        'StartDate': start_date,
                                        'EndDate': end_date
                                    }
                                } if not get_basis_month else None,
                                'SoegeAarMaanedLukketStruktur': {
                                    'SoegeAarMaanedFraKode': int(start_date.strftime('%Y%m')),
                                    'SoegeAarMaanedTilKode': int(end_date.strftime('%Y%m'))
                                } if get_basis_month else None
                            }]
                        }
                    }
                }
            }

            # Remove None values (can't send basis month if not using it)
            if not get_basis_month:
                del request_data['IndkomstOplysningPersonInddata']['IndkomstOplysningValg']['IndkomstPersonSamling']['PersonIndkomstSoegeStruktur'][0]['SoegeAarMaanedLukketStruktur']
            else:
                del request_data['IndkomstOplysningPersonInddata']['IndkomstOplysningValg']['IndkomstPersonSamling']['PersonIndkomstSoegeStruktur'][0]['SoegePeriodeLukketStruktur']

            # Make SOAP call
            # Note: The WSDL defines the operation as 'getIndkomstOplysningPersonHent'
            response = service.getIndkomstOplysningPersonHent(**request_data)

            self._log_info("Successfully received response from service")

            return response

        except Exception as e:
            self._log_error(f"Error calling eIndkomst service: {str(e)}")
            raise
