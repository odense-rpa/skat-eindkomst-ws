import wincertstore
import tempfile
from pathlib import Path
from cryptography import x509
from cryptography.hazmat.primitives import serialization
from cryptography.hazmat.backends import default_backend

def export_cert_from_store(cert_subject_contains, output_path):
    """Find and export certificate to PEM"""
    for storename in ["MY", "ROOT"]:
        try:
            with wincertstore.CertSystemStore(storename) as store:
                for cert in store.itercerts():
                    subject = cert.get_name()
                    if cert_subject_contains.lower() in subject.lower():
                        print(f"Found: {subject}")

                        # Get certificate data
                        cert_der = cert.get_encoded()

                        # Convert to PEM
                        cert_obj = x509.load_der_x509_certificate(cert_der, default_backend())
                        pem_data = cert_obj.public_bytes(serialization.Encoding.PEM)

                        # Save
                        Path(output_path).write_bytes(pem_data)
                        print(f"Exported to: {output_path}")
                        return True
        except Exception as e:
            print(f"Store {storename}: {e}")
    return False

# Export the certs
print("Exporting SKAT OIO Gateway Test...")
export_cert_from_store("SKAT OIO Gateway Test", "demo_skat.pem")

print("\nExporting OIO Gateway Klient...")
export_cert_from_store("OIO Gateway Klient", "demo_klient.pem")
