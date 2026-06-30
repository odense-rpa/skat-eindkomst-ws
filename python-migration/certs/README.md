# Certificate Location

Demo certificates are located in: `certs/demo/`

## Structure
```
certs/demo/
??? auth/
?   ??? OIO_Gateway_Klient_3_Test.p12  (Client certificate)
?   ??? readme - Klient 3.txt           (Contains password)
?   ??? ...
??? sign/
    ??? SKAT_OIO_Gateway_Test_public_base64.cer  (Server certificate)
    ??? ...
```

## To Export Certificates

If you need to regenerate PEM files:

```bash
cd python-migration

# Export from .p12 to PEM (replace PASSWORD with actual password from readme file)
python -c "from cryptography.hazmat.primitives.serialization import pkcs12, Encoding, PrivateFormat, NoEncryption; pfx=open('certs/demo/auth/OIO_Gateway_Klient_3_Test.p12','rb').read(); key,cert,_=pkcs12.load_key_and_certificates(pfx, b'PASSWORD'); open('demo_cert.pem','wb').write(cert.public_bytes(Encoding.PEM)); open('demo_key.pem','wb').write(key.private_bytes(Encoding.PEM, PrivateFormat.TraditionalOpenSSL, NoEncryption())); print('Done')"
```

## Current Configuration

The `.env` file is already configured to use the exported PEM files:
- `DEMO_AUTH_CERT_PATH` ? `demo_cert.pem`
- `DEMO_SIGNING_CERT_PATH` ? `demo_key.pem`
