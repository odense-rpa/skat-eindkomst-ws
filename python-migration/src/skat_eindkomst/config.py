"""
Configuration for SKAT eIndkomst service
"""
from dataclasses import dataclass
from typing import Optional
import os


@dataclass
class ServiceConfig:
    """Configuration for SKAT eIndkomst service"""

    dns_identity: str
    authentication_cert_name: str  # Certificate name in store (for Windows) or path (for Linux)
    signing_cert_name: str  # Certificate name in store (for Windows) or path (for Linux)
    se_nummer: str
    abonnement_type_kode: str
    abonnent_type_kode: str
    adgang_formaal_type_kode: str

    # Optional: if using certificate files directly
    authentication_cert_path: Optional[str] = None
    authentication_cert_password: Optional[str] = None
    signing_cert_path: Optional[str] = None
    signing_cert_password: Optional[str] = None

    @classmethod
    def from_env(cls, environment: str = "demo"):
        """
        Load configuration from environment variables

        Args:
            environment: Either 'demo' or 'prod'

        Returns:
            ServiceConfig instance
        """
        prefix = f"{environment.upper()}_"

        return cls(
            dns_identity=os.getenv(f"{prefix}DNS_IDENTITY", "SKAT OIO Gateway Test"),
            authentication_cert_name=os.getenv(f"{prefix}AUTH_CERT_NAME", "OIO Gateway Klient 3 Test"),
            signing_cert_name=os.getenv(f"{prefix}SIGNING_CERT_NAME", "SKAT OIO Gateway Test"),
            se_nummer=os.getenv(f"{prefix}SE_NUMMER", "19552101"),
            abonnement_type_kode=os.getenv(f"{prefix}ABONNEMENT_TYPE_KODE", "3153"),
            abonnent_type_kode=os.getenv(f"{prefix}ABONNENT_TYPE_KODE", "0750"),
            adgang_formaal_type_kode=os.getenv(f"{prefix}ADGANG_FORMAAL_TYPE_KODE", "171"),
            authentication_cert_path=os.getenv(f"{prefix}AUTH_CERT_PATH"),
            authentication_cert_password=os.getenv(f"{prefix}AUTH_CERT_PASSWORD"),
            signing_cert_path=os.getenv(f"{prefix}SIGNING_CERT_PATH"),
            signing_cert_password=os.getenv(f"{prefix}SIGNING_CERT_PASSWORD")
        )

    def __repr__(self):
        """Safe representation without exposing passwords"""
        return (
            f"ServiceConfig(dns_identity='{self.dns_identity}', "
            f"se_nummer='{self.se_nummer}', "
            f"abonnement_type_kode='{self.abonnement_type_kode}', "
            f"abonnent_type_kode='{self.abonnent_type_kode}', "
            f"adgang_formaal_type_kode='{self.adgang_formaal_type_kode}')"
        )
