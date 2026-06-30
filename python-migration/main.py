#!/usr/bin/env python3
"""
Console application for testing SKAT eIndkomst client

This is the Python equivalent of Odk.BluePrism.Skat.ConsoleApp/Program.cs
Runs in DEMO mode only (for testing purposes)
"""
import os
import sys
from datetime import datetime
from pathlib import Path

# Add src to path for development
sys.path.insert(0, str(Path(__file__).parent / "src"))

from dotenv import load_dotenv
from skat_eindkomst.config import ServiceConfig
from skat_eindkomst.client import EIndkomst
from skat_eindkomst.utils.logger import ILog


class ConsoleLogger(ILog):
    """
    Console logger that mimics the C# Spy class from Program.cs

    Outputs to console instead of Debug.WriteLine
    """

    def __init__(self):
        super().__init__("ConsoleApp")

    def Info(self, message: str):
        """Log info message"""
        print(f"INFO: {message}")

    def Error(self, message: str):
        """Log error message"""
        print(f"ERROR: {message}")


def get_env_or_default(key: str, default_value: str = "") -> str:
    """
    Get environment variable or return default value

    Args:
        key: Environment variable key
        default_value: Default value if not found

    Returns:
        Environment variable value or default
    """
    return os.getenv(key, default_value)


def get_demo_ssns():
    """
    Get demo SSNs from environment variables

    Reads DEMO_SSN_1 through DEMO_SSN_10 from .env file

    Returns:
        List of SSN strings
    """
    ssns = []
    for i in range(1, 11):
        ssn = get_env_or_default(f"DEMO_SSN_{i}")
        if ssn.strip():
            ssns.append(ssn)
    return ssns


def get_demo_config() -> ServiceConfig:
    """
    Get demo configuration from environment variables

    Returns:
        ServiceConfig for demo environment
    """
    return ServiceConfig(
        dns_identity=get_env_or_default("DEMO_DNS_IDENTITY", "SKAT OIO Gateway Test"),
        authentication_cert_name=get_env_or_default("DEMO_AUTH_CERT_NAME", "OIO Gateway Klient 3 Test"),
        signing_cert_name=get_env_or_default("DEMO_SIGNING_CERT_NAME", "SKAT OIO Gateway Test"),
        se_nummer=get_env_or_default("DEMO_SE_NUMMER", "19552101"),
        abonnement_type_kode=get_env_or_default("DEMO_ABONNEMENT_TYPE_KODE", "3153"),
        abonnent_type_kode=get_env_or_default("DEMO_ABONNENT_TYPE_KODE", "0750"),
        adgang_formaal_type_kode=get_env_or_default("DEMO_ADGANG_FORMAAL_TYPE_KODE", "171"),
        authentication_cert_path=get_env_or_default("DEMO_AUTH_CERT_PATH"),
        signing_cert_path=get_env_or_default("DEMO_SIGNING_CERT_PATH")
    )


def run_demo():
    """
    Run demo test

    This mirrors the C# RunDemo() method from Program.cs
    """
    demo_ssns = get_demo_ssns()

    if not demo_ssns:
        print("WARNING: No demo SSNs found in .env file. Using default or add DEMO_SSN_1, DEMO_SSN_2, etc.")
        demo_ssns = ["3004861026"]  # Default from C# code

    # Create logger
    logger = ConsoleLogger()

    # Create config
    config = get_demo_config()

    # Create eIndkomst client
    e = EIndkomst(config=config, environment="demo", logger=logger)

    # Get first SSN
    ssn = demo_ssns[0]
    user = get_env_or_default("DEMO_USER", "SystemNameTest")

    # Generate request identifier
    ident = f"TEST XXXXXX {datetime.now().strftime('%Y%m%d_%H%M%S.%f')[:-3]}"

    print("=" * 80)
    print("Kalder eindkomst IndkomstOplysningPersonHent TEST")
    print(f"Bruger: {user}, ssn: {ssn}, ID: {ident}")
    if demo_ssns:
        print(f"Available test SSNs: {', '.join(demo_ssns)}")
    print("=" * 80)

    try:
        # Call service
        datatable = e.indkomst_oplysning_person_hent(
            ssn=ssn,
            worker_id=user,
            start_date=datetime(2023, 1, 1),
            end_date=datetime(2023, 12, 30),
            request_id=ident,
            get_basis_month=False
        )

        print("\nDone deal...")
        print(f"\nRetrieved {len(datatable)} records:")

        # Display results
        if not datatable.empty:
            print("\n" + "=" * 80)
            print(datatable.to_string())
            print("=" * 80)

            # Save to CSV
            output_file = f"eindkomst_{ssn}_{datetime.now().strftime('%Y%m%d_%H%M%S')}.csv"
            datatable.to_csv(output_file, index=False, encoding='utf-8-sig')
            print(f"\nData saved to: {output_file}")
        else:
            print("No data returned from service")

    except Exception as ex:
        print(f"ERROR: {str(ex)}")
        import traceback
        traceback.print_exc()


def main():
    """Main entry point"""

    # Load .env file from project root (parent directory)
    env_path = Path(__file__).parent.parent / ".env"

    if env_path.exists():
        print(f"Loading environment from: {env_path}")
        load_dotenv(env_path)
    else:
        print(f"Warning: .env file not found at {env_path}")
        print("Continuing with system environment variables...")

    print("\n" + "=" * 80)
    print("SKAT eIndkomst Python Console Application")
    print("Python Migration from C#/.NET Framework 4.7.2")
    print("=" * 80 + "\n")

    # Run demo only
    run_demo()

    # Prompt for exit (skip if running in non-interactive mode)
    if sys.stdin.isatty():
        print("\nPress Enter to exit...")
        input()


if __name__ == "__main__":
    main()
