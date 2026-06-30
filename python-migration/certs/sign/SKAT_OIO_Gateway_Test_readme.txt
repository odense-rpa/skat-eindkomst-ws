Keystore type: PKCS12
Keystore provider: SunJSSE

Your keystore contains 1 entry

Alias name: skat oio gateway test
Creation date: Mar 2, 2023
Entry type: PrivateKeyEntry
Certificate chain length: 3
Certificate[1]:
Owner: C=DK, OID.2.5.4.97=NTRDK-19552101, O=Skatteforvaltningen, SERIALNUMBER=UI:DK-O:G:ba2fa92e-6d23-4ef2-8139-8ee90a297cbe, CN=SKAT OIO Gateway Test
Issuer: C=DK, O=Den Danske Stat, OU=Test - cti, CN=Den Danske Stat OCES udstedende-CA 1
Serial number: 1b2b38b030b95e07c1b5fdf04be7c81a76d386ef
Valid from: Thu Mar 02 19:22:45 CET 2023 until: Sun Mar 01 19:22:44 CET 2026
Certificate fingerprints:
	 SHA1: BB:5B:D6:AC:F8:3C:AE:47:3B:4F:60:27:34:D7:27:3F:F0:1B:03:00
	 SHA256: 8A:AA:E8:66:8B:13:5F:7B:0B:F7:64:55:31:CE:72:97:39:96:FC:C4:D7:A4:50:87:2E:4E:77:BD:17:01:96:DE
Signature algorithm name: RSASSA-PSS
Subject Public Key Algorithm: 3072-bit RSA key (3)
Version: {10}

Extensions: 

#1: ObjectId: 1.3.6.1.5.5.7.1.3 Criticality=false
0000: 30 2D 30 2B 06 08 2B 06   01 05 05 07 0B 02 30 1F  0-0+..+.......0.
0010: 06 07 04 00 8B EC 49 01   02 30 14 86 12 68 74 74  ......I..0...htt
0020: 70 73 3A 2F 2F 75 69 64   2E 67 6F 76 2E 64 6B     ps://uid.gov.dk


#2: ObjectId: 1.3.6.1.5.5.7.1.1 Criticality=false
AuthorityInfoAccess [
  [
   accessMethod: caIssuers
   accessLocation: URIName: http://ca1.cti-gov.dk/oces/issuing/1/cacert/issuing.cer
, 
   accessMethod: ocsp
   accessLocation: URIName: http://ca1.cti-gov.dk/ocsp
]
]

#3: ObjectId: 2.5.29.35 Criticality=false
AuthorityKeyIdentifier [
KeyIdentifier [
0000: 7F 28 9F D9 71 99 42 E2   75 E7 D7 35 76 2E 4D 08  .(..q.B.u..5v.M.
0010: 25 6D 76 5E                                        %mv^
]
]

#4: ObjectId: 2.5.29.19 Criticality=true
BasicConstraints:[
  CA:false
  PathLen: undefined
]

#5: ObjectId: 2.5.29.31 Criticality=false
CRLDistributionPoints [
  [DistributionPoint:
     [URIName: http://ca1.cti-gov.dk/oces/issuing/1/crl/issuing.crl]
]]

#6: ObjectId: 2.5.29.32 Criticality=false
CertificatePolicies [
  [CertificatePolicyId: [0.4.0.2042.1.1]
[]  ]
  [CertificatePolicyId: [1.2.208.169.1.1.1.3.7]
[]  ]
]

#7: ObjectId: 2.5.29.15 Criticality=true
KeyUsage [
  DigitalSignature
  Non_repudiation
  Key_Encipherment
]

#8: ObjectId: 2.5.29.14 Criticality=false
SubjectKeyIdentifier [
KeyIdentifier [
0000: 74 BF 56 71 1B 8A E7 85   3D B2 A3 89 35 AA 91 7B  t.Vq....=...5...
0010: 5F 74 A0 6B                                        _t.k
]
]

Certificate[2]:
Owner: C=DK, O=Den Danske Stat, OU=Test - cti, CN=Den Danske Stat OCES udstedende-CA 1
Issuer: C=DK, O=Den Danske Stat, OU=Test - cti, CN=Den Danske Stat OCES rod-CA
Serial number: 734d70a62e521cd17c644c99a0900e1d59e061ae
Valid from: Fri Mar 12 08:42:59 CET 2021 until: Mon Mar 10 08:42:58 CET 2031
Certificate fingerprints:
	 SHA1: 72:34:7B:AE:17:45:68:89:42:D4:9D:FF:5F:2C:80:53:8B:5B:02:3B
	 SHA256: 57:C7:85:23:6A:D7:C9:C9:86:B9:4D:49:AB:DA:FD:1B:95:5C:55:09:33:8F:A4:3A:7C:E0:D6:60:88:C8:AF:9B
Signature algorithm name: RSASSA-PSS
Subject Public Key Algorithm: 3072-bit RSA key (3)
Version: {10}

Extensions: 

#1: ObjectId: 1.3.6.1.5.5.7.1.1 Criticality=false
AuthorityInfoAccess [
  [
   accessMethod: caIssuers
   accessLocation: URIName: http://ca1.cti-gov.dk/oces/root/cacert/root.cer
, 
   accessMethod: ocsp
   accessLocation: URIName: http://ca1.cti-gov.dk/ocsp
]
]

#2: ObjectId: 2.5.29.35 Criticality=false
AuthorityKeyIdentifier [
KeyIdentifier [
0000: 39 DC DE DE D0 90 26 47   A0 E0 C6 6E 49 7F 26 F2  9.....&G...nI.&.
0010: A9 2F F6 1B                                        ./..
]
]

#3: ObjectId: 2.5.29.19 Criticality=true
BasicConstraints:[
  CA:true
  PathLen:0
]

#4: ObjectId: 2.5.29.31 Criticality=false
CRLDistributionPoints [
  [DistributionPoint:
     [URIName: http://ca1.cti-gov.dk/oces/root/crl/root.crl]
]]

#5: ObjectId: 2.5.29.15 Criticality=true
KeyUsage [
  Key_CertSign
  Crl_Sign
]

#6: ObjectId: 2.5.29.14 Criticality=false
SubjectKeyIdentifier [
KeyIdentifier [
0000: 7F 28 9F D9 71 99 42 E2   75 E7 D7 35 76 2E 4D 08  .(..q.B.u..5v.M.
0010: 25 6D 76 5E                                        %mv^
]
]

Certificate[3]:
Owner: C=DK, O=Den Danske Stat, OU=Test - cti, CN=Den Danske Stat OCES rod-CA
Issuer: C=DK, O=Den Danske Stat, OU=Test - cti, CN=Den Danske Stat OCES rod-CA
Serial number: 573f57e67530f1a0777dfbc69f090438d3360256
Valid from: Thu Jan 28 10:49:25 CET 2021 until: Mon Jan 22 10:49:24 CET 2046
Certificate fingerprints:
	 SHA1: 0E:99:4B:E0:FA:C7:E8:59:85:4C:39:49:95:59:99:F4:E3:03:7D:22
	 SHA256: D1:BC:EC:41:D7:AC:AE:93:2B:7D:FE:66:A8:B7:34:1C:A7:59:52:C8:66:4F:CC:EF:4C:4A:E2:15:0F:95:C5:EC
Signature algorithm name: RSASSA-PSS
Subject Public Key Algorithm: 4096-bit RSA key (3)
Version: {10}

Extensions: 

#1: ObjectId: 1.3.6.1.5.5.7.1.1 Criticality=false
AuthorityInfoAccess [
  [
   accessMethod: caIssuers
   accessLocation: URIName: http://ca1.cti-gov.dk/oces/root/cacert/root.cer
, 
   accessMethod: ocsp
   accessLocation: URIName: http://ca1.cti-gov.dk/ocsp
]
]

#2: ObjectId: 2.5.29.19 Criticality=true
BasicConstraints:[
  CA:true
  PathLen:2147483647
]

#3: ObjectId: 2.5.29.31 Criticality=false
CRLDistributionPoints [
  [DistributionPoint:
     [URIName: http://ca1.cti-gov.dk/oces/root/crl/root.crl]
]]

#4: ObjectId: 2.5.29.15 Criticality=true
KeyUsage [
  Key_CertSign
  Crl_Sign
]

#5: ObjectId: 2.5.29.14 Criticality=false
SubjectKeyIdentifier [
KeyIdentifier [
0000: 39 DC DE DE D0 90 26 47   A0 E0 C6 6E 49 7F 26 F2  9.....&G...nI.&.
0010: A9 2F F6 1B                                        ./..
]
]



*******************************************
*******************************************


