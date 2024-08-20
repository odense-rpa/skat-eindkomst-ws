# Odk.BluePrism


## Odk.BluePrism.Skat

Implementerer en C# proxy klasse til SKAT's eIndkomst webservices. Ved kald af denne proxy, håndteres resultatet ved at blive mappet til et DataTable objekt. Denne version af proxy'en mapper kun endpointet: "IndkomstOplysningPersonHent". 
Et build resulterer i en assembly(library) som installeres i BluePrism, og derved kan tilgås i et code-stage.

Nedenfor refereres web-servicen, som "service".


### Forudsætning for brug af proxy'en.

Adgang oprettes ved at der underskrives et abonnement i Udviklings- og Forenklingsstyrelsen, som er en del af Skatteforvaltningen.   

Skabelonen til denne aftale kan findes på Skats hjemmeside.

Ved indgåelse af aftalen, tildeles abonnementsformåls- og typekoder som medfører hjemmel til at foretage opslag i eIndkomst – eksempelvis:

AbonnentTypeKode 		"xxxx"              (Udleveres ved indgåelse af aftalen)

AbonnementTypeKode 		"3153"					(Indkomstafhængige ydelser)

AdgangFormaalTypeKode 	"171"     			(Aktiv beskæftigelsesindsats – Flekslønstilskud)

Med formålet, kaldet ”171”, er der hjemmel til at foretage opslag som vedrører borgere som modtager Fleksløntilskud.

Proxy'en skal konfigureres med disse koder inden kaldet foretages.

### WebService endpoint "IndkomstOplysningPersonHent"

Proxy'en understøtter p.t. kun et endpoint til servicen - "IndkomstOplysningPersonHent".

Der er nogle afhængigheder som skal opfyldes før man kan danne og anvende proxy’en. 
-	Der skal være indgået aftale som nævnt overfor
-	Der skal være adgang til de seneste WSDL-er i hhv. demo og produktionsmiljøet i eIndkomst.
-	Din organisations organisationscertifikat skal være autoriseret i TastSelv som ”revisor”.
-	Organisationscertifikat og signeringscertifikat skal være gyldige og placeret i computerens certifikat-lager. 

Servicen udtrækker ale indberettede indkomstoplysninger på en person i et givet dato-interval.
Oplysningerne dækker bl.a. A- og B-indkomst, indeholdt skat, feriepengeoplysninger fra det eller de ansættelsesforhold personen har haft i udsøgningsperioden, SE nummer på den indberettende virksomhed m.m. 
Servicen returnerer en mængde af blanketter, som indeholder feltkoder, indberetningstyper og -arter samt de værdier der er indberettet herunder. Blanketterne danner et hierarki.

Servicen er synkron, dvs. at når forespørgsel afsendes vil svaret blive modtaget i samme kald.

De nærmere detaljer om servicen findes på Skats hjemmeside.

Servicen kaldes via en proxy genereret ud fra WSDL-skema filer. 
Har man brug for at generere proxien på ny, kan WSDL-filerme tilgås online på Skats såkaldte eIndkomst-Wiki.
Det er vigtigt her at sikre sig, man bruger de seneste WSDL-skema filer, som understøtter TIN (Tax Identification Number).

Se nærmere under punktet "Eksempel på service-proxy generering" nedenfor.

#### Tekniske detaljer vedr. servicen

Servicen benytter SOAP1.1, og WS-Security 1.0 over HTTPS

Kommunikationsformen svarer til Microsofts “Message Security with Mutual Certificates”

Overordnet set er der 3 konkrete sikkerheds-niveauer som skal overholdes:

1)	Kryptering
2)	Autentifikation
3)	Signering

##### Kryptering

Krypteringen sker på transport-laget (HTTPS).

##### Autentifikation

Autentifikation foretages på message-laget, med din organisations gældende organisationscertifikat. Organisationscertifikatet skal være installeret i computerens certifikat-lager for den ”Aktuelle bruger”.
Skat validerer alle kald til servicen op mod dette certifikat. Det er vigtigt at proceduren for aktivering af organisationscertifikatet er foretaget korrekt som beskrevet på Skats side vedr. eIndkomst. 

##### Signering

Signering foretages på message-laget. 
Signeringen sker med Skatteforvaltningens offentlige del af signatur-certifikat ”SKAT OIO Gateway Prod”. Certifikatet skal installeres i computerens certifikat-lager for ”Lokal Computer”.

Dele af kaldets indhold er signeret med certifikatet og dele af svarets indhold er signeret med Skats certifikat.
Selve signaturen ligger i et felt i SOAP-headeren. 

Bemærk: Organisationscertifikatet skal være i formatet OCES3.

### Eksempel på service-proxy generering:

Servicen kaldes via en proxy genereret ud fra WSDL-skema filer. 

Har man brug for at generere proxy'en på ny, kan WSDL-filerme tilgås online på Skats såkaldte eIndkomst-Wiki.
Det er vigtigt at sikre sig, der bruges de seneste WSDL-skema filer, som understøtter TIN (Tax Identification Number).

Service proxy'en skal altid genereres manuelt med værktøjet 'svcutil.exe' fra kommandoprompten.

#### Produktion

`svcutil https://eksternwiki.skat.dk/services/prodtin/eIndkomst10/IndkomstOplysningPersonHent.wsdl /out:EIndkomstProxy.cs /namespace:*,dk.skat.eindkomst /syncOnly /noConfig`

#### Demo

`svcutil https://eksternwiki.skat.dk/services/demotin/eIndkomst10/IndkomstOplysningPersonHent.wsdl /out:EIndkomstProxyDemo.cs /namespace:*,dk.skat.eindkomst /syncOnly /noConfig`


*Bemærk:*

Når service proxy'ens C# fil er genereret, SKAL attributten 'System.ServiceModel.ServiceContractAttribute' tilføjes følgende 'ProtectionLevel = System.Net.Security.ProtectionLevel.Sign'.

Det skal ske manuelt, og hver gang man kører værktøjet.

`[System.ServiceModel.ServiceContractAttribute(
    Namespace="http://rep.oio.dk/skat.dk/eindkomst/", 
    ConfigurationName="eindkomst.prod.IndkomstOplysningPersonHentServicePortType", 
    ProtectionLevel = System.Net.Security.ProtectionLevel.Sign)
]`

Kopier den dannede fil til den respektive mappe under "Services".


## Konfiguration

ServiceConfig

Ved hjælp af et ServiceConfig objekt vil man kunne konfigurere de parametre som servicen bruger i kontakten til eIndkomst. Dette objekt skal altid angives ved brug af servicen, da man via dette definerer under hvilke abonnementsforhold man kalder eIndkomst, hvilke certifikater der skal benyttes med videre.

Objektet kan bruges som argument til proxy’ens constructor – som eksempel:


` var sconfig = new ServiceConfig
 {
     AbonnementTypeKode = "...",
     AdgangFormaalTypeKode = "...",
     AbonnentTypeKode = "...",
     AuthenticationCert = "...",
     SigningCert = "...",
     DNSIdentity = "...",
     SENummer = "..."
 };`

 `EIndkomst e = new EIndkomst(sconfig);`


Det er oftes parametrene AbonnementTypeKode, AbonnentTypeKode og AdgangFormaalTypeKode som ændres i tilfælde af at der er aftalt andre forbindelsesaftaler med eIndkomst end fleksløntilskud.

I tilfælde af at certifkaterne udløber, eller skal udskiftes er det vigtigt at ændre AuthenticationCertificateName, og SigningCertificateName respektivt.

## Udsøgning af lønperiode, eller basismåned

Servicen udstiller to public metoder:

e.IndkomstOplysningPersonHent( ... ) som returnerer et DataTable objekt.

e.EIndkomstPersonHentKlient( ... ) som returnerer et IndkomstOplysningPersonHent_OType objekt (rå service data)

Ved kald til servicen kan der angives to former for udsøgning af datoperiode.

### Lønperiode

Ved lønperiode forstås alle indkomstoplysninger for en borger hvor indberetteren har angivet udbetalingens lønperiode inden for den start- og slutdato som udsøges.

Lønperioder udsøges som standard af servicen. Det sker ved at der i metode kaldet IndkomstOplysningPersonHent implicit er angivet en parameter ”getbasismonth” til false. Parameteren kan også angives eksplicit.

### Basismåned

Ved basismåned forstås alle indkomstoplysninger for en borger hvor indberetteren har angivet dispositionsdatoen inden for en periode som består af startår, og startmåned som slutår og slutmåned. Altså i formatet ”ÅÅÅÅMD”.

Udsøgningen i services sker ved at der i metode kaldet IndkomstOplysningPersonHent eksplicit angives parameteren ”getbasismonth” til true, samt man – som i lønperiode – angiver start- og slutdato. Systemet udregner der ved et startår, -måned og slutår, -måned på baggrund af start/slutdatoens år og måned – uagtet hvilken dag i datoen der er angivet.

#### Eksempel på dato.

01.01.2020 – 15.03.2020 omregnes til ”202001”, og ”202003”.

En udsøgning på basismåned er essentiel i tilfælde af man ønsker alle indberetninger og reguleringer af tidligere lønperioder medtaget i resultatet fra eIndkomst. En regulering adskiller sig typisk ved at have en dispositionsdato på et senere tidspunkt en lønperiodens slutdato.

## Skat/eIndkomst dokumentation

 
 https://info.skat.dk/data.aspx?oid=4247&vid=219530 

 https://eksternwiki.skat.dk/eindkomst2 (WSDL's m.m.)
 
 https://info.skat.dk/data.aspx?oid=44296 (service driftlogs)


## Odk.BluePrism.Skat.ConsoleApp

En konsol-applikation til direkte test af proxy'en. 

## Odk.BluePrism.Skat.Tests

Unittests. 
 

