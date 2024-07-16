using dk.skat.eindkomst;
using Odk.BluePrism.Skat.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Odk.BluePrism.Skat.Parsers
{
    public class eIndkomstParser : IeIndkomstParser
    {
        private readonly ILog log;

        public eIndkomstParser(ILog logger)
        {
            log = logger;
        }

        public eIndkomstParser()
        {

        }

        private decimal ToDecimal(string txt)
        {
            return Convert.ToDecimal(txt, System.Globalization.CultureInfo.InvariantCulture);
        }


        private void LogInfo(string txt)
        {
            if (log != null)
            {
                log.Info(txt);
            }
        }


        public DataTable ParseResultToDataTable(IndkomstOplysningPersonHent_OType personHentType)
        {
            if (personHentType is null)
            {
                throw new ArgumentNullException(nameof(personHentType));
            }

            var table = BuildDataTable();



            IndkomstOplysningPersonHent_OTypeIndkomstPersonUddataIndkomstOplysningPersonSamling o = personHentType.IndkomstPersonUddata?.Item as IndkomstOplysningPersonHent_OTypeIndkomstPersonUddataIndkomstOplysningPersonSamling;
            if (o == null)
                return table;
            var s = o.IndkomstOplysningPersonStruktur.FirstOrDefault();

            for (var i = 0; i < s.IndkomstOplysningSamling.Length; i++)
            {
                var samling = s.IndkomstOplysningSamling[i];
                var senr = samling.IndberetningPligtigVirksomhedStruktur.IndberetningPligtigVirksomhed.VirksomhedSENummerIdentifikator;
                LogInfo(String.Format("Indberetning fra: {0}", senr));

                for (var j = 0; j < samling.IndkomstLoenPeriodeSamling.Length; j++)
                {
                    var løn = samling.IndkomstLoenPeriodeSamling[j];
                    var angivelsesperiode = løn.AngivelsePeriodeStruktur.AngivelsePeriode;
                    var periodestart = DateTime.Parse(angivelsesperiode.Items[0].ToString()).ToShortDateString();
                    var periodeslut = DateTime.Parse(angivelsesperiode.Items[1].ToString()).ToShortDateString();

                    var dispositionsDato = ((løn.IndkomstPersonGruppeDispositionDatoSpecified) ? løn.IndkomstPersonGruppeDispositionDato : DateTime.MinValue).ToShortDateString();

                    var row = table.NewRow();
                    row["Udbetalt den"] = dispositionsDato;
                    row["Lønperiode start"] = periodestart;
                    row["Lønperiode slut"] = periodeslut;
                    row["Lønperiode"] = periodestart + " " + periodeslut;
                    row["SE-nummer"] = senr;

                    LogInfo("Dispositionsdato: " + dispositionsDato);
                    LogInfo("Lønperiode: " + periodestart + " - " + periodeslut);



                    for (var k = 0; k < løn.AngivelseBlanketIndholdStruktur.AngivelseOplysningSamling.Length; k++)
                    {
                        var angivelse = løn.AngivelseBlanketIndholdStruktur.AngivelseOplysningSamling[k];

                        DoBlanket11000(row, angivelse);
                    }
                    table.Rows.Add(row);
                }
            }

            if (table.HasErrors)
            {
                throw new Exception("DataTable has errors");
            }
            else
                table.AcceptChanges();

            return table;
        }

        // blanket 11000 (Generellse indkomstoplysninger)
        private void DoBlanket11000(DataRow row, AngivelseNiveau1Type angivelse)
        {
            // setup datatable schema
            var internalDataTableName = "11000";
            var internalDatatable = new DataTable(internalDataTableName);
            internalDatatable.Columns.Add(CreateDefaultStringColumn("key"));
            internalDatatable.Columns.Add(CreateDefaultStringColumn("value"));

            //internalDatatable.Columns.Add(CreateDefaultStringColumn("001"));   // SE nummer på indberetter / lønservicebureau
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("002"));   //
            //internalDatatable.Columns.Add(CreateDefaultStringColumn("003"));   //
            //internalDatatable.Columns.Add(CreateDefaultStringColumn("004"));   //
            //internalDatatable.Columns.Add(CreateDefaultStringColumn("006"));   //
            //internalDatatable.Columns.Add(CreateDefaultStringColumn("007"));   //
            //internalDatatable.Columns.Add(CreateDefaultStringColumn("008"));   //
            //internalDatatable.Columns.Add(CreateDefaultStringColumn("009"));   //
            //internalDatatable.Columns.Add(CreateDefaultStringColumn("159"));   //
            //internalDatatable.Columns.Add(CreateDefaultStringColumn("010"));   //
            //internalDatatable.Columns.Add(CreateDefaultStringColumn("011"));   //
            //internalDatatable.Columns.Add(CreateDefaultStringColumn("012"));   //
            //internalDatatable.Columns.Add(CreateDefaultStringColumn("013"));   //
            //internalDatatable.Columns.Add(CreateDefaultStringColumn("014"));   //
            //internalDatatable.Columns.Add(CreateDefaultStringColumn("015"));   //
            //internalDatatable.Columns.Add(CreateDefaultStringColumn("016"));   //
            //internalDatatable.Columns.Add(CreateDefaultStringColumn("018"));   //
            //internalDatatable.Columns.Add(CreateDefaultStringColumn("151"));   //
            //internalDatatable.Columns.Add(CreateDefaultStringColumn("191"));   //

            var blanketnr = angivelse.BlanketNummerIdentifikator;

            LogInfo(blanketnr + " ******************************************** ");



            foreach (var feltsamling in angivelse.AngivelseFeltSamling)
            {
                string blanketFeltNummer = feltsamling.BlanketFeltEnhedStruktur.BlanketFeltNummerIdentifikator;
                string angivelsesTekst = feltsamling.AngivelseFeltIndholdTekst;

                var internalRow = internalDatatable.NewRow();
                internalRow["key"] = blanketFeltNummer;
                internalRow["value"] = angivelsesTekst;

                LogInfo(String.Format("{0} {1}", blanketFeltNummer, angivelsesTekst));
                string value = "";
                switch (blanketFeltNummer)
                {
                    case "100000000000000001":
                        //internalRow["001"] = angivelsesTekst;                                                
                        break;
                    case "100000000000000002":
                        //internalRow["002"] = ToDecimal(angivelsesTekst);
                        break;
                    case "100000000000000003":
                        //var periodestart = DateTime.Parse(angivelsesTekst);
                        //internalRow["003"] = angivelsesTekst;
                        break;
                    case "100000000000000004":
                        //internalRow["004"] = angivelsesTekst;
                        break;
                    case "100000000000000005":
                        break;
                    case "100000000000000006":
                        //internalRow["006"] = angivelsesTekst;
                        break;                                          // CVR nummer for ovennævnte SEnummer
                    case "100000000000000007":
                        //internalRow["007"] = angivelsesTekst;
                        break;
                    case "100000000000000008":
                        //internalRow["008"] = angivelsesTekst;
                        break;
                    case "100000000000000009":
                        //internalRow["009"] = angivelsesTekst;
                        break;
                    case "100000000000000010":
                        //internalRow["010"] = angivelsesTekst;
                        break;
                    case "100000000000000011":                          // Indberetningsart (I = indberetning, T = tibageførsel)
                        //internalRow["011"] = angivelsesTekst;
                        break;
                    case "100000000000000012":                          // Indkomsttype
                        //internalRow["012"] = angivelsesTekst;
                        row["Indkomstart"] = value = GetIndkomstTypeFromCode(angivelsesTekst);
                        row["Indkomsttypekode"] = angivelsesTekst;
                        LogInfo(" - " + blanketFeltNummer + " " + angivelsesTekst + " parsed: " + value);
                        break;
                    case "100000000000000013":
                        //internalRow["013"] = angivelsesTekst;
                        break;
                    case "100000000000000014":
                        //internalRow["014"] = angivelsesTekst;
                        break;
                    case "100000000000000015":
                        //internalRow["015"] = angivelsesTekst;
                        break;
                    case "100000000000000016":
                        //internalRow["016"] = angivelsesTekst;
                        var sub = $"{angivelsesTekst.Substring(0, 4)}-{angivelsesTekst.Substring(4)}";
                        row["Skattemåned"] = sub;
                        break;
                    case "100000000000000018":
                        //internalRow["018"] = angivelsesTekst;
                        break;
                    case "100000000000000151":
                        //internalRow["151"] = angivelsesTekst;
                        row["Rettelse til tidligere indberetning"] = (angivelsesTekst == "1");
                        if (angivelsesTekst == "1")
                            LogInfo(" - OBS rettelse til tidligere periode");
                        break;
                    case "100000000000000159":
                        //internalRow["159"] = angivelsesTekst;
                        break;
                    case "100000000000000191":
                        //internalRow["191"] = angivelsesTekst;
                        break;
                    default:
                        {
                            LogInfo(String.Format("Felt nummer ikke matchet: {0} {1}", blanketFeltNummer, angivelsesTekst));
                            break;
                        }
                }
                internalDatatable.Rows.Add(internalRow);
            }


            if (internalDatatable.HasErrors)
            {
                throw new Exception($"DataTable {internalDataTableName} has errors");
            }
            else
                internalDatatable.AcceptChanges();

            row[internalDataTableName] = internalDatatable;

            // blanket 16000
            var blnk = angivelse.UnderAngivelseSamling.Where(b => b.BlanketNummerIdentifikator == "16000");
            if (blnk.Count() > 1)
                throw new Exception("Cant parse more than one blanket 16000");
            DoBlanket16000(row, blnk);
        }

        protected void DoBlanket16000(DataRow row, IEnumerable<AngivelseNiveau2Type> blanket16000)
        {
            // setup datatable schema
            var internalDataTableName = "16000";
            var internalDatatable = new DataTable(internalDataTableName);
            internalDatatable.Columns.Add(CreateDefaultStringColumn("key"));
            internalDatatable.Columns.Add(CreateDefaultStringColumn("value"));

            //internalDatatable.Columns.Add(CreateDefaultStringColumn("150"));   // Markering for om E101 attest forefindes
            //internalDatatable.Columns.Add(CreateDefaultStringColumn("019"));   // Indtægtsart
            //internalDatatable.Columns.Add(CreateDefaultStringColumn("020"));   // Produktionsenhedsnummer
            //internalDatatable.Columns.Add(CreateDefaultStringColumn("021"));   // Markering for tilbageførsel

            foreach (var b in blanket16000)
            {

                var feltid1 = b.BlanketNummerIdentifikator;
                LogInfo(feltid1 + " ---- ");
                foreach (var underfelt in b.AngivelseFeltSamling)
                {
                    var blanketFeltNummer = underfelt.BlanketFeltEnhedStruktur.BlanketFeltNummerIdentifikator;
                    var angivelsesTekst = underfelt.AngivelseFeltIndholdTekst;

                    var internalRow = internalDatatable.NewRow();
                    internalRow["key"] = blanketFeltNummer;
                    internalRow["value"] = angivelsesTekst;

                    LogInfo(blanketFeltNummer + " " + angivelsesTekst);
                    string value = "";
                    switch (blanketFeltNummer)
                    {
                        case "100000000000000019":                      // indtægtsart
                            row["Indkomstart_19"] = value = GetIndkomstArtFromCode(Convert.ToUInt16(angivelsesTekst).ToString("D4"));
                            row["Indkomstartkode"] = angivelsesTekst;
                            //internalRow["019"] = angivelsesTekst;
                            LogInfo(" - " + blanketFeltNummer + " " + angivelsesTekst + " parsed: " + value);
                            break;
                        case "100000000000000020":                      // PE-nummer
                            row["Produktionsenhed"] = value = angivelsesTekst;
                            LogInfo(" - " + blanketFeltNummer + " " + angivelsesTekst + " parsed: " + value);
                            //internalRow["020"] = angivelsesTekst;
                            break;
                        case "100000000000000021":                      // TODO: Markering for tilbageførsel  
                            row["Tilbageførsel"] = (angivelsesTekst == "1");
                            //internalRow["021"] = angivelsesTekst;
                            break;
                        case "100000000000000150":                      // Markering for om 101 attest forefindes
                            //internalRow["150"] = angivelsesTekst;
                            break;
                        default:
                            LogInfo(String.Format("Felt nummer ikke matchet: {0} {1}", blanketFeltNummer, angivelsesTekst));
                            break;
                    }
                    internalDatatable.Rows.Add(internalRow);
                }



                var blanket16001 = b.UnderAngivelseSamling.Where(c => c.BlanketNummerIdentifikator == "16001");
                DoBlanket16001(row, blanket16001);

                var blanket16200 = b.UnderAngivelseSamling.Where(c => c.BlanketNummerIdentifikator == "16200");
                DoBlanket16200(row, blanket16200);

                var blanket16201 = b.UnderAngivelseSamling.Where(c => c.BlanketNummerIdentifikator == "16201");
                DoBlanket16201(row, blanket16201);

                var blanket16202 = b.UnderAngivelseSamling.Where(c => c.BlanketNummerIdentifikator == "16202");
                DoBlanket16202(row, blanket16202);

                var blanket16300 = b.UnderAngivelseSamling.Where(c => c.BlanketNummerIdentifikator == "16300");
                DoBlanket16300(row, blanket16300);
            }

            if (internalDatatable.HasErrors)
            {
                throw new Exception($"DataTable {internalDataTableName} has errors");
            }
            else
                internalDatatable.AcceptChanges();

            row[internalDataTableName] = internalDatatable;

        }

        protected void DoBlanket16001(DataRow row, IEnumerable<AngivelseNiveau3Type> blanket16001)
        {
            // setup datatable schema
            var internalDataTableName = "16001";
            var internalDatatable = new DataTable(internalDataTableName);
            internalDatatable.Columns.Add(CreateDefaultStringColumn("key"));
            internalDatatable.Columns.Add(CreateDefaultStringColumn("value"));

            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("056"));   // Ingen beregning af befordringsfradrag på basis af produktionsenhedsnummer
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("057"));   // A-indkomst, hvoraf der betales AM-bidrag 
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("058"));   // A-indkomst, hvoraf der betales AM-bidrag 
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("059"));   // Indeholdt A-skat
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("060"));   // Indeholdt AM-bidrag
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("062"));   // Bilagsnummer tilsvar A-skat
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("065"));   // Værdi af fri bil tilrådighed
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("066"));   // Værdi af fri kost og logi
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("067"));   // Indskud til arbejdsgiveradministreret ordning i svensk pensionsinstitut
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("068"));   // AM-bidrag af pensionsordning i svensk pensionsinstitut
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("070"));   // B-indkomst, hvoraf der betales AM-bidrag
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("071"));   // B-indkomst, hvoraf der ikke betales AM-bidrag
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("072"));   // Hædersgaver
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("073"));   // "Naturalieydelser (Tidligere 'Naturalieydelse fra fonde og foreninger')"
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("074"));   // Supplerende oplysninger om naturalieydelse fra fonde og foreninger
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("075"));   // Sats for ATP-bidrag
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("076"));   // ATP-bidrag
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("078"));   // Skattefri rejse og befordringsgodtgørelse
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("079"));   // Værdi af fri helårsbolig
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("080"));   // Værdi af fri sommerbolig her i landet
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("081"));   // Værdi af fri lystbåd
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("082"));   // Værdi af fri TV-licens
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("083"));   // Værdi af fri telefon
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("084"));   // Ydet personalelån
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("085"));   // Fri sommerbolig i udlandet
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("086"));   // Fri befordring mellem hjem og arbejde mv.
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("089"));   // Ansattes årlige andel vedrørende pc-ordning
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("090"));   // Supplerende oplysninger til kode 68
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("091"));   // Jubilæumsgratiale og fratrædelsesgodtgørelse
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("092"));   // "Jubilæumsgratiale og fratrædelsesgodtgørelse indbetalt på pensionsordning"
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("093"));   // Jubilæumsgratiale og fratrædelsesgodtgørelse udbetalt som tingsgaver
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("094"));   // Antal sødage
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("095"));   // Tilbagebetaling af kontanthjælp
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("096"));   // Løntimer
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("097"));   // Bruttoløn (bruttoindkomst)
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("098"));   // Bruttoferiepenge 
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("104"));   // Barselsfondskode
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("157"));   // Opsparet Søgne- og helligdagsbetaling
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("158"));   // Opsparet feriefridag - omregnet til kroner
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("167"));   // Godkendelsesnummer for udenlandsk pensionsordning
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("168"));   // Skattefri del af udbetaling fra godkendt udenlandsk pensionsordning
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("169"));   // Værdi af multimediebeskatning
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("170"));   // Andre personalegoder
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("171"));   // Pension fritaget for udligningsskat
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("172"));   // Bidrag til obligatorisk udl. social sikring
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("182"));   // Værdi af personalelån
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("183"));   // A-skattepligtig sundhedsforsikring/behandling
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("184"));   // Værdi af fri telefon mm.
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("185"));   // Værdi af andre personalegoder, der overstiger bundgrænse
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("186"));   // Værdi af andre personalegoder, uden bundgrænse
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("188"));   // Pensionsindskud – med bortseelse
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("189"));   // Gruppeliv/sundhedsforsikring i pensionsindskud
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("190"));   // Sundhedsforsikring i pensionsindskud
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("192"));   // Bruttoindskud i medarbejderinvesteringsselskab
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("193"));   // AM-bidrag af indskud i medarbejderinvesteringsselskab
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("194"));   // CVR/SE-nr. på medarbejderinvesteringsselskab
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("196"));   // RUT-nr. på udenlandsk arbejdsgiver vedr. AFU
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("197"));   // Lønmodtagers pensionsandel
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("198"));   // Arbejdsgivers pensionsandel
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("199"));   // Lønmodtager - eget ATP-bidrag
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("200"));   // Ingen forhold mellem løn og timer
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("206"));   // Indbetaling på udenlandsk pension - Grønland
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("207"));   // Skat af udenlandsk pension - Grønland
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("208"));   // A-indkomst fra aldersopsparing
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("209"));   // A-skat af aldersopsparing
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("215"));   // OP-bidrag 
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("216"));   // Offentligt tilskud og godtgørelse
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("217"));   // A-indkomst, udbetalt som feriepenge
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("220"));   // SE-nr. på FGO, der udbetaler feriepenge



            //  Alle felter i blanket 16001 (Indkomstoplysninger)
            foreach (var underfelt2 in blanket16001)
            {
                var feltid2 = underfelt2.BlanketNummerIdentifikator;
                LogInfo(feltid2 + " ---- ");
                LogInfo("");



                foreach (var n3 in underfelt2.AngivelseFeltSamling)
                {
                    var blanketFeltNummer = n3.BlanketFeltEnhedStruktur.BlanketFeltNummerIdentifikator;
                    var angivelsesTekst = n3.AngivelseFeltIndholdTekst;

                    var internalRow = internalDatatable.NewRow();
                    internalRow["key"] = blanketFeltNummer;
                    internalRow["value"] = angivelsesTekst;
                    LogInfo(blanketFeltNummer + " " + angivelsesTekst);

                    switch (blanketFeltNummer)
                    {
                        case "100000000000000056":  // Ingen beregning af befordringsfradrag på basis af produktionsenhedsnummer
                            //internalRow["056"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000057":  // A-indkomst inkl. AMbidrag
                            row["A-indkomst"] = ToDecimal(angivelsesTekst);   // decimal tegn er punktum internalRow["057"] = 
                            break;
                        case "100000000000000058":  // A-indkomst uden AM-bidrag
                            row["A-indkomst uden AM-bidrag"] = ToDecimal(angivelsesTekst);   // decimal tegn er punktum internalRow["058"] = 
                            break;
                        case "100000000000000059":  // Indeholdt A-skat
                            row["A-skat"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000060":  // Indeholdt AM-bidrag
                            row["heraf AM-bidrag"] = ToDecimal(angivelsesTekst);    //internalRow["060"] = 
                            break;
                        case "100000000000000062":  // 
                            //internalRow["062"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000065":  // 
                            //internalRow["065"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000067":  // 
                            //internalRow["067"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000068":  // 
                            //internalRow["068"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000070":  // B-indkomst inkl. AM-bidrag
                            //internalRow["070"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000071":  // B-indkomst hvor der ikke betales AM-bidrag
                            row["B-indkomst"] = ToDecimal(angivelsesTekst);     //internalRow["071"] = 
                            break;
                        case "100000000000000072":  // 
                            //internalRow["072"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000073":  // 
                            //internalRow["073"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000074":  // 
                            //internalRow["074"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000075":  // 
                            //internalRow["075"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000076":  // ATP-bidrag
                            row["heraf Atp-bidrag"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000078":  //
                            //internalRow["078"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000079":  // 
                            //internalRow["079"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000080":  // 
                            //internalRow["080"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000081":
                            //internalRow["081"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000082":
                            //internalRow["082"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000083":
                            //internalRow["083"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000084":
                            //internalRow["084"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000085":
                            //internalRow["085"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000086":
                            //internalRow["086"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000089":
                            //internalRow["089"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000090":
                            //internalRow["090"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000091":
                            //internalRow["091"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000092":
                            //internalRow["092"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000093":
                            //internalRow["093"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000094":
                            //internalRow["094"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000095":
                            //internalRow["095"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000096":  // Løntimer
                            row["Timer"] = ToDecimal(angivelsesTekst);  //internalRow["096"] = 
                            break;
                        case "100000000000000097":
                            //internalRow["097"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000098":
                            decimal bruttoferiepenge = ToDecimal(angivelsesTekst);
                            row["Bruttoferiepenge"] = row["heraf henlagt til feriepenge"] = bruttoferiepenge;   //= internalRow["098"] 
                            break;
                        case "100000000000000104":
                            //internalRow["104"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000157":
                            //decimal opsparetsh = ToDecimal(angivelsesTekst);
                            //internalRow["157"] = opsparetsh;
                            break;
                        case "100000000000000158":
                            //internalRow["158"] = ToDecimal(angivelsesTekst); 
                            break;
                        case "100000000000000167":
                            //internalRow["167"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000168":
                            //internalRow["168"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000169":
                            //internalRow["169"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000170":
                            //internalRow["170"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000171":
                            //internalRow["171"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000172":
                            //internalRow["172"] = ToDecimal(angivelsesTekst);
                            break;
                        //case "100000000000000176":
                        //    // TODO: hvordan skal feriepenge håndteres hvis der i samme blanket er både felt 176, 178? 
                        //    decimal nettoferiepenge = ToDecimal(angivelsesTekst);
                        //    internalRow["176"] = nettoferiepenge;
                        //    row["Nettoferiepenge"] = nettoferiepenge;
                        //    break;
                        //case "100000000000000177":
                        //    internalRow["177"] = ToDecimal(angivelsesTekst);
                        //    break;
                        //case "100000000000000178":
                        //    internalRow["178"] = ToDecimal(angivelsesTekst);
                        //    nettoferiepenge = ToDecimal(angivelsesTekst);
                        //    row["Nettoferiepenge"] = nettoferiepenge;
                        //    break;
                        case "100000000000000182":
                            //internalRow["182"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000183":
                            //internalRow["183"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000184":
                            //internalRow["184"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000185":
                            //internalRow["185"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000186":
                            //internalRow["186"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000188":
                            //internalRow["188"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000189":
                            //internalRow["189"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000190":
                            //internalRow["190"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000192":
                            //internalRow["192"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000193":
                            //internalRow["193"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000194":
                            //internalRow["194"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000196":
                            //internalRow["196"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000197":
                            //internalRow["197"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000198":
                            //internalRow["198"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000199":              // Lønmodtager ATP eget bidrag (1/3) af felt 076
                            row["Atp-bidrag lønmodtager"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000200":
                            //internalRow["200"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000206":
                            //internalRow["206"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000207":
                            //internalRow["207"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000208":
                            //internalRow["208"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000209":
                            //internalRow["209"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000215":
                            //internalRow["215"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000216":
                            //internalRow["216"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000217":
                            //internalRow["217"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000220":
                            //internalRow["220"] = ToDecimal(angivelsesTekst);
                            break;
                        default:
                            LogInfo(String.Format("Felt nummer ikke matchet: {0} {1}", blanketFeltNummer, angivelsesTekst));
                            break;
                    }

                    internalDatatable.Rows.Add(internalRow);

                }


                var blanket16002 = underfelt2.UnderAngivelseSamling.Where(c => c.BlanketNummerIdentifikator == "16002");
                DoBlanket16002(row, blanket16002);

            }
            if (internalDatatable.HasErrors)
            {
                throw new Exception($"DataTable {internalDataTableName} has errors");
            }
            else
                internalDatatable.AcceptChanges();

            row[internalDataTableName] = internalDatatable;
        }

        private void DoBlanket16002(DataRow row, IEnumerable<AngivelseNiveau4Type> blanket16002)
        {
            // setup datatable schema
            var internalDataTableName = "16002";
            var internalDatatable = new DataTable(internalDataTableName);
            internalDatatable.Columns.Add(CreateDefaultStringColumn("key"));
            internalDatatable.Columns.Add(CreateDefaultStringColumn("value"));

            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("160"));   // 
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("161"));   // 
            //internalDatatable.Columns.Add(CreateDefaultStringColumn("162"));   // 
            //internalDatatable.Columns.Add(CreateDefaultStringColumn("163"));   // 
            //internalDatatable.Columns.Add(CreateDefaultStringColumn("164"));   // 
            //internalDatatable.Columns.Add(CreateDefaultStringColumn("165"));   // 
            //internalDatatable.Columns.Add(CreateDefaultStringColumn("166"));   // 
            //internalDatatable.Columns.Add(CreateDefaultStringColumn("173"));   // 
            //internalDatatable.Columns.Add(CreateDefaultStringColumn("174"));   // 
            //internalDatatable.Columns.Add(CreateDefaultStringColumn("175"));   // 

            foreach (var underfelt in blanket16002)
            {
                var feltid = underfelt.BlanketNummerIdentifikator;
                LogInfo(feltid + " ---- ");
                LogInfo("");
                foreach (var n in underfelt.AngivelseFeltSamling)
                {
                    var blanketFeltNummer = n.BlanketFeltEnhedStruktur.BlanketFeltNummerIdentifikator;
                    var angivelsesTekst = n.AngivelseFeltIndholdTekst;
                    var internalRow = internalDatatable.NewRow();
                    internalRow["key"] = blanketFeltNummer;
                    internalRow["value"] = angivelsesTekst;

                    LogInfo(blanketFeltNummer + " " + angivelsesTekst);


                    switch (blanketFeltNummer)
                    {
                        case "100000000000000160":  // Optjente feriedage for timelønnede 
                            //internalRow["160"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000161":  // Restferiedage for fratrædende funktionærer
                            //internalRow["161"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000162":  // Optjeningsår for fratrædende funktionærer
                            //internalRow["162"] = angivelsesTekst;
                            break;
                        case "100000000000000163":  // Fratrædelsesdato for fratrædende funktionærer
                            //internalRow["163"] = angivelsesTekst;
                            break;
                        case "100000000000000164":  // FeriepengeUdbetaler
                            //internalRow["164"] = angivelsesTekst;
                            break;
                        case "100000000000000165":  // Bruttoferiepenge for fratrædende funktionærer
                            //internalRow["165"] = (angivelsesTekst);
                            break;
                        case "100000000000000166":  // Bruttoferiepenge for timelønnede
                            //internalRow["166"] = (angivelsesTekst);
                            break;
                        case "100000000000000173":  // FeriepengeUdbetaler
                            //internalRow["173"] = angivelsesTekst;
                            break;
                        case "100000000000000174":  // FeriepengeUdbetaler
                            //internalRow["174"] = angivelsesTekst;
                            break;
                        case "100000000000000175":  // FeriepengeUdbetaler
                            //internalRow["175"] = angivelsesTekst;
                            break;
                        default:
                            LogInfo(String.Format("Felt nummer ikke matchet: {0} {1}", blanketFeltNummer, angivelsesTekst));
                            break;
                    }
                    internalDatatable.Rows.Add(internalRow);
                }


            }
            if (internalDatatable.HasErrors)
            {
                throw new Exception($"DataTable {internalDataTableName} has errors");
            }
            else
                internalDatatable.AcceptChanges();

            row[internalDataTableName] = internalDatatable;

        }

        protected void DoBlanket16300(DataRow row, IEnumerable<AngivelseNiveau3Type> blanket16300)
        {
            // setup datatable schema
            var internalDataTableName = "16300";
            var internalDatatable = new DataTable(internalDataTableName);
            internalDatatable.Columns.Add(CreateDefaultStringColumn("key"));
            internalDatatable.Columns.Add(CreateDefaultStringColumn("value"));

            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("058"));
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("070"));
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("071"));
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("078"));
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("095"));
            //internalDatatable.Columns.Add(CreateDefaultStringColumn("201"));
            //internalDatatable.Columns.Add(CreateDefaultStringColumn("202"));
            //internalDatatable.Columns.Add(CreateDefaultStringColumn("203"));
            //internalDatatable.Columns.Add(CreateDefaultStringColumn("204"));
            //internalDatatable.Columns.Add(CreateDefaultStringColumn("205"));

            //if (blanket16300.Count() > 1)
            //    throw new Exception("Cant parse more than one blanket 16300");
            //  Alle felter i blanket 16300 (Ydelsesrefusionsoplysninger)
            foreach (var underfelt in blanket16300)
            {
                var feltid = underfelt.BlanketNummerIdentifikator;
                LogInfo(feltid + " ---- ");
                LogInfo("");
                foreach (var n in underfelt.AngivelseFeltSamling)
                {
                    var blanketFeltNummer = n.BlanketFeltEnhedStruktur.BlanketFeltNummerIdentifikator;
                    var angivelsesTekst = n.AngivelseFeltIndholdTekst;

                    var internalRow = internalDatatable.NewRow();
                    internalRow["key"] = blanketFeltNummer;
                    internalRow["value"] = angivelsesTekst;

                    LogInfo(blanketFeltNummer + " " + angivelsesTekst);


                    switch (blanketFeltNummer)
                    {

                        case "100000000000000058":  // A-indkomst hvoraf der ikke betales AM-bidrag
                            //internalRow["058"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000070":  // B-indkomst, hvoraf der betales AM-bidrag
                            //internalRow["070"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000071":  // B-indkomst, hvoraf der ikke betales AM-bidrag
                            //internalRow["071"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000078":  // Skattefri rejse og befordringsgodtgørelse
                            //internalRow["078"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000095":  // Tilbagebetaling af kontanthjælp
                            //internalRow["095"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000201":  // Kontonummer - ikke Fælleskommunal
                            //internalRow["201"] = angivelsesTekst;
                            break;
                        case "100000000000000202":  // Kontonummer - Fælleskommunal
                            //internalRow["202"] = angivelsesTekst;
                            break;
                        case "100000000000000203":  // Ydelsesperiode for ydelsesrefusion
                            //internalRow["203"] = angivelsesTekst;
                            break;
                        case "100000000000000204":  // Periode - for refusion/tilskud til borger eller arbejdsgiver
                            //internalRow["204"] = angivelsesTekst;
                            break;
                        case "100000000000000205":  // Udløsende CPR-nr ved refusion/tilskud til arbejdsgiver
                            //internalRow["205"] = angivelsesTekst;
                            break;
                        default:
                            LogInfo(String.Format("Felt nummer ikke matchet: {0} {1}", blanketFeltNummer, angivelsesTekst));
                            break;
                    }

                    internalDatatable.Rows.Add(internalRow);

                }
            }
            if (internalDatatable.HasErrors)
            {
                throw new Exception($"DataTable {internalDataTableName} has errors");
            }
            else
                internalDatatable.AcceptChanges();

            row[internalDataTableName] = internalDatatable;
        }

        protected void DoBlanket16202(DataRow row, IEnumerable<AngivelseNiveau3Type> blanket16202)
        {
            // setup datatable schema
            var internalDataTableName = "16202";
            var internalDatatable = new DataTable(internalDataTableName);
            internalDatatable.Columns.Add(CreateDefaultStringColumn("key"));
            internalDatatable.Columns.Add(CreateDefaultStringColumn("value"));

            //internalDatatable.Columns.Add(CreateDefaultStringColumn("177"));
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("179"));
            //internalDatatable.Columns.Add(CreateDefaultStringColumn("180"));
            //internalDatatable.Columns.Add(CreateDefaultStringColumn("181"));
            //internalDatatable.Columns.Add(CreateDefaultStringColumn("187"));
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("213"));
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("214"));


            //if (blanket16202.Count() > 1)
            //    throw new Exception("Cant parse more than one blanket 16202");
            //  Alle felter i blanket 16202 (Bruttoferiepengeoplysninger)
            foreach (var underfelt in blanket16202)
            {
                var feltid = underfelt.BlanketNummerIdentifikator;
                LogInfo(feltid + " ---- ");
                LogInfo("");
                foreach (var n in underfelt.AngivelseFeltSamling)
                {
                    var blanketFeltNummer = n.BlanketFeltEnhedStruktur.BlanketFeltNummerIdentifikator;
                    var angivelsesTekst = n.AngivelseFeltIndholdTekst;

                    var internalRow = internalDatatable.NewRow();
                    internalRow["key"] = blanketFeltNummer;
                    internalRow["value"] = angivelsesTekst;

                    LogInfo(blanketFeltNummer + " " + angivelsesTekst);


                    switch (blanketFeltNummer)
                    {

                        case "100000000000000177":  // Optjente feriedage for timelønnede 
                            //internalRow["177"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000179":  // Restferiedage for fratrædende funktionærer
                            //internalRow["179"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000180":  // Optjeningsår for fratrædende funktionærer
                            //internalRow["180"] = angivelsesTekst;
                            break;
                        case "100000000000000181":  // Fratrædelsesdato for fratrædende funktionærer
                            //internalRow["181"] = angivelsesTekst;
                            break;
                        case "100000000000000187":  // FeriepengeUdbetaler
                            //internalRow["187"] = angivelsesTekst;
                            break;
                        case "100000000000000213":  // Bruttoferiepenge for fratrædende funktionærer
                            //internalRow["213"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000214":  // Bruttoferiepenge for timelønnede
                            //internalRow["214"] = ToDecimal(angivelsesTekst);
                            break;
                        default:
                            LogInfo(String.Format("Felt nummer ikke matchet: {0} {1}", blanketFeltNummer, angivelsesTekst));
                            break;
                    }
                    internalDatatable.Rows.Add(internalRow);
                }
            }
            if (internalDatatable.HasErrors)
            {
                throw new Exception($"DataTable {internalDataTableName} has errors");
            }
            else
                internalDatatable.AcceptChanges();

            row[internalDataTableName] = internalDatatable;
        }

        protected void DoBlanket16201(DataRow row, IEnumerable<AngivelseNiveau3Type> blanket16201)
        {
            // setup datatable schema
            var internalDataTableName = "16201";
            var internalDatatable = new DataTable(internalDataTableName);
            internalDatatable.Columns.Add(CreateDefaultStringColumn("key"));
            internalDatatable.Columns.Add(CreateDefaultStringColumn("value"));

            //internalDatatable.Columns.Add(CreateDefaultStringColumn("187"));
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("211"));
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("212"));

            //if (blanket16201.Count() > 1)
            //    throw new Exception("Cant parse more than one blanket 16201");
            //  Alle felter i blanket 16201 (feriepengeoplysninger til LFM)
            foreach (var underfelt in blanket16201)
            {
                var feltid = underfelt.BlanketNummerIdentifikator;
                LogInfo(feltid + " ---- ");
                LogInfo("");


                foreach (var n in underfelt.AngivelseFeltSamling)
                {
                    var blanketFeltNummer = n.BlanketFeltEnhedStruktur.BlanketFeltNummerIdentifikator;
                    var angivelsesTekst = n.AngivelseFeltIndholdTekst;

                    var internalRow = internalDatatable.NewRow();
                    internalRow["key"] = blanketFeltNummer;
                    internalRow["value"] = angivelsesTekst;

                    LogInfo(blanketFeltNummer + " " + angivelsesTekst);

                    switch (blanketFeltNummer)
                    {
                        case "100000000000000187":      // feriepengeudbetaler (se nr)
                            //internalRow["187"] = angivelsesTekst;
                            break;
                        case "100000000000000211":      // Bruttoferiepenge for fortsættende funktionær
                            //internalRow["211"] = ToDecimal(angivelsesTekst);
                            break;
                        case "100000000000000212":      // Feriedage for fortsættende funktionær
                            //internalRow["212"] = ToDecimal(angivelsesTekst);
                            break;
                        default:
                            LogInfo(String.Format("Felt nummer ikke matchet: {0} {1}", blanketFeltNummer, angivelsesTekst));
                            break;

                    }
                    internalDatatable.Rows.Add(internalRow);
                }

            }

            if (internalDatatable.HasErrors)
            {
                throw new Exception($"DataTable {internalDataTableName} has errors");
            }
            else
                internalDatatable.AcceptChanges();

            row[internalDataTableName] = internalDatatable;
        }

        protected void DoBlanket16200(DataRow row, IEnumerable<AngivelseNiveau3Type> blanket16200)
        {
            // setup datatable schema
            var internalDataTableName = "16200";
            var internalDatatable = new DataTable(internalDataTableName);
            internalDatatable.Columns.Add(CreateDefaultStringColumn("key"));
            internalDatatable.Columns.Add(CreateDefaultStringColumn("value"));

            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("176"));
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("177"));
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("178"));
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("179"));
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("180"));
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("181"));
            //internalDatatable.Columns.Add(CreateDefaultDecimalColumn("187"));

            //if (blanket16200.Count() > 1)
            //    throw new Exception("Cant parse more than one blanket 16200");
            //  Alle felter i blanket 16200 (Nettoferiepengeoplysninger)
            foreach (var underfelt in blanket16200)
            {
                var feltid = underfelt.BlanketNummerIdentifikator;
                LogInfo(feltid + " ---- ");
                LogInfo("");


                foreach (var n in underfelt.AngivelseFeltSamling)
                {
                    var blanketFeltNummer = n.BlanketFeltEnhedStruktur.BlanketFeltNummerIdentifikator;
                    var angivelsesTekst = n.AngivelseFeltIndholdTekst;

                    var internalRow = internalDatatable.NewRow();
                    internalRow["key"] = blanketFeltNummer;
                    internalRow["value"] = angivelsesTekst;

                    LogInfo(blanketFeltNummer + " " + angivelsesTekst);

                    var value = ToDecimal(angivelsesTekst);

                    switch (blanketFeltNummer)
                    {
                        case "100000000000000176":      // Nettoferiepenge for timelønnede  (varchar(18) inkl. evt. fortegn)
                            // TODO: hvordan skal feriepenge håndteres hvis der i samme blanket er både felt 176, 178? 
                            row["Nettoferiepenge"] = value; //internalRow["176"] = 
                            break;
                        case "100000000000000177":      // Optjente feriedage for timelønnede (varchar(10) inkl. evt. fortegn)
                            //internalRow["177"] = value;
                            break;
                        case "100000000000000178":      // Nettoferiepenge for fratrædende funktionærer (varchar(18) inkl. evt. fortegn)
                            row["Nettoferiepenge"] = value; //internalRow["178"] = 
                            break;
                        case "100000000000000179":      // Restferiedage for fratrædende funktionærer   (varchar(10) inkl. evt. fortegn)
                            //internalRow["179"] = value;
                            break;
                        case "100000000000000180":      // Optjeningsår for fratrædende funktionærer    (numerisk)
                            //internalRow["180"] = value;
                            break;
                        case "100000000000000181":      // Fratrædelsesdato for fratrædende funktionærer (numerisk)
                            //internalRow["181"] = value;
                            break;
                        case "100000000000000182":
                        case "100000000000000183":
                        case "100000000000000184":
                        case "100000000000000185":
                        case "100000000000000186":
                            break;
                        case "100000000000000187":      // FeriepengeUdbetaler (numerisk)
                            //internalRow["187"] = value;
                            break;
                        default:
                            LogInfo(string.Format("Felt nummer ikke matchet: {0} {1}", blanketFeltNummer, angivelsesTekst));
                            break;
                    }
                    internalDatatable.Rows.Add(internalRow);
                }




            }
            if (internalDatatable.HasErrors)
            {
                throw new Exception($"DataTable {internalDataTableName} has errors");
            }
            else
                internalDatatable.AcceptChanges();

            row[internalDataTableName] = internalDatatable;
        }

        private DataTable BuildDataTable()
        {
            var table = new DataTable("Result");
            // Declare datatable schema.
            table.Columns.Add(CreateDefaultStringColumn("SE-nummer"));
            table.Columns.Add(CreateDefaultStringColumn("Udbetaler"));
            table.Columns.Add(CreateDefaultStringColumn("Indkomstart"));
            table.Columns.Add(CreateDefaultStringColumn("Lønperiode"));
            table.Columns.Add(CreateDefaultDateTimeColumn("Udbetalt den"));
            table.Columns.Add(CreateDefaultStringColumn("Skattemåned"));
            table.Columns.Add(CreateDefaultDecimalColumn("Bruttoindkomst"));
            table.Columns.Add(CreateDefaultDecimalColumn("heraf Atp-bidrag"));
            table.Columns.Add(CreateDefaultDecimalColumn("Atp-bidrag lønmodtager"));
            table.Columns.Add(CreateDefaultDecimalColumn("A-indkomst"));
            table.Columns.Add(CreateDefaultDecimalColumn("A-indkomst uden AM-bidrag"));
            table.Columns.Add(CreateDefaultDecimalColumn("B-indkomst"));
            table.Columns.Add(CreateDefaultDecimalColumn("A-skat"));
            table.Columns.Add(CreateDefaultDecimalColumn("heraf AM-bidrag"));
            table.Columns.Add(CreateDefaultDecimalColumn("heraf henlagt til feriepenge"));
            table.Columns.Add(CreateDefaultDecimalColumn("heraf henlagt til s/h-dage"));
            table.Columns.Add(CreateDefaultDecimalColumn("heraf henlagt til feriefridage"));
            table.Columns.Add(CreateDefaultDecimalColumn("Timer"));
            table.Columns.Add(CreateDefaultStringColumn("Produktionsenhed"));
            table.Columns.Add(CreateDefaultDateTimeColumn("Lønperiode start"));
            table.Columns.Add(CreateDefaultDateTimeColumn("Lønperiode slut"));
            table.Columns.Add(CreateDefaultStringColumn("Indkomstart_19"));
            table.Columns.Add(CreateDefaultStringColumn("Indkomsttypekode"));
            table.Columns.Add(CreateDefaultStringColumn("Indkomstartkode"));
            table.Columns.Add(CreateDefaultDecimalColumn("Bruttoferiepenge"));
            table.Columns.Add(CreateDefaultDecimalColumn("Nettoferiepenge"));
            table.Columns.Add(CreateDefaultBooleanColumn("Rettelse til tidligere indberetning"));
            table.Columns.Add(CreateDefaultBooleanColumn("Tilbageførsel"));
            table.Columns.Add(new DataColumn("11000", typeof(DataTable)));
            table.Columns.Add(new DataColumn("16000", typeof(DataTable)));
            table.Columns.Add(new DataColumn("16001", typeof(DataTable)));
            table.Columns.Add(new DataColumn("16002", typeof(DataTable)));
            table.Columns.Add(new DataColumn("16200", typeof(DataTable)));
            table.Columns.Add(new DataColumn("16201", typeof(DataTable)));
            table.Columns.Add(new DataColumn("16202", typeof(DataTable)));
            table.Columns.Add(new DataColumn("16300", typeof(DataTable)));
            return table;
        }

        private static DataColumn CreateDefaultDecimalColumn(string columnname)
        {
            var dataColumn = new DataColumn(columnname, typeof(decimal));
            dataColumn.DefaultValue = 0;
            return dataColumn;
        }
        private static DataColumn CreateDefaultStringColumn(string columnname)
        {
            var dataColumn = new DataColumn(columnname, typeof(string));
            dataColumn.DefaultValue = string.Empty;
            return dataColumn;
        }

        private static DataColumn CreateDefaultDateTimeColumn(string columnname)
        {
            var dataColumn = new DataColumn(columnname, typeof(DateTime));
            return dataColumn;
        }

        private static DataColumn CreateDefaultBooleanColumn(string columnname)
        {
            var dataColumn = new DataColumn(columnname, typeof(bool));
            return dataColumn;
        }

        /// <summary>
        /// Mapper værdisættet af indtægtsarter (kode 68) fra eIndkomst til tekst 
        /// 
        /// Se. eIndkomst Teknisk vejledning (afsnit 4.9.1) for specifikation af værdisæt
        /// </summary>
        /// <param name="angivelsesTekst"></param>
        /// <returns></returns>
        private string GetIndkomstArtFromCode(string angivelsesTekst)
        {
            switch (angivelsesTekst)
            {
                case "0002":
                    return "Dagpenge, der er B-indkomst";
                case "0003":
                    return "Dagpenge (ved sygdom og ulykke)";
                case "0004":
                    return "Éngangsbeløb - opsat pension, jf. § 15 d, stk. 4 i lov om social pension";
                case "0005":
                    return "Arbejdsløshedsdagpenge";
                case "0006":
                    return "Folke-, førtids- og seniorpension";
                case "0007":
                    return "Lønmodtagernes Garantifond";
                case "0008":
                    return "Udbetalinger fra private arbejdsløshedsforsikringer";
                case "0009":
                    return "Arbejdsmarkedets TillægsPension - ATP";
                case "0010":
                    return "Ældrecheck";
                case "0011":
                    return "Biblioteksafgift";
                case "0012":
                    return "SP løbende udbetalinger";
                case "0013":
                    return "Efterløn - løbende udbetaling";
                case "0014":
                    return "Kursusbeløb/kørselsgodtgørelse udbetalt af arbejdsløshedskassen";
                case "0015":
                    return "Strejke- og lockoutgodtgørelse";
                case "0016":
                    return "Anden understøttelse fra a-kasserne";
                case "0018":
                    return "Rentetilskud til statsgaranteret studielån";
                case "0019":
                    return "Uddannelsesydelse til ledige";
                case "0020":
                    return "Udbetalt pension omfattet af Pensionsbeskatningslovens §15B (opsparingsordning for sportsudøvere)";
                case "0021":
                    return "Ikke skattepligtig indkomst til Danmark udbetalt af Danida m.fl.";
                case "0024":
                    return "Delpension";
                case "0025":
                    return "Tjenestemandspension";
                case "0026":
                    return "Forskerordning";
                case "0027":
                    return "DIS-søindkomst, anden Udenrigsfart- DIS (Dansk Internationalt Skibsregister)";
                case "0028":
                    return "DAS-søindkomst - Uden for begrænset fart - Udenrigsfart - Indenrigsfart, rutetrafik fra havn til havn på mindst 50 sømil - Stenfiskere/sandsugere DAS (Dansk Almindelig Skibsregister)";
                case "0030":
                    return "A-indkomst for dagplejere, der ønsker standardfradrag efter LL § 9 H";
                case "0034":
                    return "Søindkomst, Færøerne/Grønland. Anvendes når sømanden er begrænset skattepligtig efter kildeskattelovens § 2, stk. 2 og har optjent sin løn ved tjeneste om bord på et dansk skib, som sejler uden for begrænset fart. Indkomstarten anvendes når sømanden bor på Færøerne eller Grønland, og rederiet i en måned ikke har modtaget refusion efter § 10 i SØBL";
                case "0036":
                    return "Ydelser efter serviceloven, hvori der kan ske lønindeholdelse";
                case "0037":
                    return "Skattepligtig gruppelivsforsikring, som ikke er betalt via bortseelsesberettiget træk i løn.\r\nSkal bruges af fagforeninger når præmien betales af fagforeningskontingent og af pensionsudbydere, når præmien ikke betales af indskud, der stammer fra løntræk - fx via bonus på en fradragsberettiget pensionsordning.\r\nBeløbet indgår ikke i grundlaget for beskæftigelsesfradrag.\r\n(Præmie, der betales af indskud, der stammer fra bortseelsesberettiget træk i løn til pensionsordning, henhører under felt 0091.)";
                case "0038":
                    return "Anden skattepligtig ydelse - kommunalt udbetalt -  se nærmere i indberetningsvejledningen";
                case "0039":
                    return "Efterlevelseshjælp";
                case "0041":
                    return "Skattepligtig kontant- og engangshjælp (aktivlov § 25), kontantydelse og aktiveringsydelse";
                case "0042":
                    return "Skattepligtig revalideringsydelse (aktivlov §52)";
                case "0044":
                    return "Bidragspligtig pension";
                case "0045":
                    return "Vederlag for afløsning af pensionstilsagn";
                case "0046":
                    return "Fratrædelsesgodtgørelse efter gamle regler";
                case "0048":
                    return "Forskerordning";
                case "0049":
                    return "Skattefri uddannelsesydelse (LL § 31 stk.3, nr.3 og 4).";
                case "0050":
                    return "Købe- og tegningsretter til aktier samt Aktier/Anparter efter LL § 16 skal indberettes i felt 36. I de tilfælde, hvor retten ikke er værdisat skal der indberettes et kryds i felt 40";
                case "0051":
                    return "Købe- og tegningsretter LL § 28";
                case "0052":
                    return "Køberetter udnyttet efter LL §7H";
                case "0056":
                    return "Søindkomst, Færøerne/Grønland. Anvendes når sømanden er begrænset skattepligtig efter kildeskattelovens § 2, stk. 2 og har optjent sin løn ved tjeneste om bord på et dansk skib, som sejler uden for begrænset fart. Indkomstarten anvendes når sømanden bor på Færøerne eller Grønland, og rederiet i en måned har modtaget refusion efter § 10 i SØBL";
                case "0057":
                    return "DIS-indkomst, Begrænset fart - DIS - Færge-DIS (Rutetrafik fra havn til havn på under  50 sømil)";
                case "0060":
                    return "Personalegoder til direktør";
                case "0061":
                    return "Personalegoder til hovedaktionær samt udloddet fri bil som udbytte";
                case "0062":
                    return "Computer, anskaffet ved reduktion i bruttoløn";
                case "0064":
                    return "iilbagebetaling af bidrag vedr. Efterløns- og fleksydelsesbidrag (Engangsudbetaling) efter 30. juni 2007 (L154 - Lov nr 347 af 18. april 2007 - Forhøjelse af aldersgrænser og tilbagebetaling af efterløns- og fleksydelsesbidrag).\r\nBeløb vedr. indbetalinger inden 2002 skal indberettes i felt 70 og beløb vedr. indbetalinger fra og med 2002 skal i felt 71.";
                case "0069":
                    return "Corona - selvstændig kompensation og andre hjælpepakker";
                case "0070":
                    return "Skattepligtige offentlige tilskud";
                case "0071":
                    return "Skattefrie offentlige tilskud";
                case "0072":
                    return "Rettelser indsendt af SU styrelsen";
                case "0073":
                    return "Skattefrit soldaterlegat";
                case "0074":
                    return "Anden pension. \r\nAlle pensionsudbetalinger, som ikke har en anden indtægtsart (kode 68), skal indberettes med indtægtsart 0074 i samme indberetning som selve pensionsindberetningen";
                case "0075":
                    return "Nettogevinst ved visse spil";
                case "0076":
                    return "Værdi af naturaliepræmier";
                case "0077":
                    return "Pengepræmier";
                case "0078":
                    return "Barselsdagpenge som A-indkomst.\r\nBarselsdagpenge som B-indkomst, herunder kompensation efter barselsudligningsloven";
                case "0079":
                    return "Indkomst ved selvstændigt erhverv.\r\nBruges fx ved domstolenes indberetning af salær til beskikkede forsvarere";
                case "0080":
                    return "Indskud til aldersopsparing. Aldersopsparingsudbyderes årlige indberetning af indskud, når beløbet skal til beskatning som B-indkomst";
                case "0081":
                    return "Indskud til kapitalpension. Kapitalpensionsudbyderes årlige indberetning af indskud, når beløbet skal til beskatning som B-indkomst.";
                case "0082":
                    return "Vederlag for privat pasning. Dækker vederlag til privat pasningsordning for børn hvor pasningen foregår i vederlagsmodtagers eget hjem og  kommunen yder tilskud til forældrene efter dagtilbudslovens § 80";
                case "0083":
                    return "Kommunalt flekslønstilskud udbetalt til lønmodtager";
                case "0084":
                    return "Kommunalt ressourceforløbsydelse";
                case "0086":
                    return "Fleksydelse - løbende udbetaling";
                case "0087":
                    return "Uddannelseshjælp og aktivitetstillæg - udbetales af kommunen";
                case "0088":
                    return "Udbytte fra medarbejderinvesteringsselskab";
                case "0089":
                    return "Udlodning og avance fra medarbejderinvesteringsselskab";
                case "0097":
                    return "Skattefrie uddelinger/legater fra fonde (B-indkomst). ";
                case "0099":
                    return "Skattepligtige uddelinger fra skattepligtige fonde til almennyttige formål og andre formål, der giver fonden ret til fradrag for uddelingen. Gældende fra 05.10.2016";
                case "0100":
                    return "Skattepligtige uddelinger fra skattepligtige fonde til ikke-almennyttige formål, der ikke giver fonden ret til fradrag for uddelingen. Kun 80 % af værdien af uddelingen beskattes hos modtager";
                case "0101":
                    return "Købe- eller tegningsretter til aktier, skattefrie efter LL § 7 P eller med udskudt beskatning efter LL § 28";
                case "0102":
                    return "Vederlag i forbindelse med offentligt ombud og hverv.";
                case "0103":
                    return "Sygedagpenge der er A-indkomst og udbetales af kommuner og arbejdsgivere. Gældende for perioder i 2017 og senere";
                case "0104":
                    return "G-dagesgodtgørelse for ledighedsdage fra arbejdsgivere ved ledighed og arbejdsfordeling, samt kompensation fra kommunen for ikke anvist seniorjob.";
                case "0105":
                    return "Tabt arbejdsfortjeneste efter § 42 i lov om social service - vedr. børn.";
                case "0106":
                    return "Plejevederlag efter § 120 i lov om social service - vedr. døende.";
                case "0107":
                    return "Éngangstillæg for opsat pension (udbetales af Udbetaling Danmark), jf. § 15F, stk. 3 i lov om social pension. Tæller ikke med i topskattegrundlag";
                case "0108":
                    return "Skattefri rejsegodtgørelse jf. Ligningslovens § 9A";
                case "0109":
                    return "Skattefri befordringsgodtgørelse (kørselsgodtgørelse) jf. Ligningslovens § 9B";
                case "0110":
                    return "Invalidepension\r\nUdbetaling af invalidepension indtil opnåelse af folkepensionsalderen, jf. § 1 a i lov om social pension";
                case "0111":
                    return "Rateforsikring ved invaliditet\r\nUdbetaling af rateforsikring i tilfælde af forsikredes invaliditet";
                case "0112":
                    return "Rateopsparing ved nedsat arbejdsevne\r\nUdbetaling af rateopsparing i tilfælde af kontohaverens varigt nedsatte arbejdsevne, som efter reglerne i lov om social pension berettiger kontohaveren til at oppebære førtidspension";
                case "0113":
                    return "Ægtefælle og samleverpension\r\nUdbetaling af ægtefælle- eller samleverpension";
                case "0114":
                    return "Pension udbetalt til efterladte og børnepension\r\nEr i praksis udbetaling til børn og andre personer, der er indsat som begunstigede på en pensionsordning";
                case "0115":
                    return "Ekstra personligt tillæg - mediecheck.\r\nUdbetalt efter § 14 e, i lov om social pension.";
                case "0116":
                    return "Tidlig pension - udbetalt af Udbetaling Danmark efter Lov om social pension";
                case "0127":
                    return "Udenlandsk DIS-søindkomst jf. SØBL § 11a";
                case "0128":
                    return "Udenlandsk DAS-søindkomst jf. SØBL § 11b";
                case "0170":
                    return "OP-bidrag - tillægsrefusion";
                case "0171":
                    return "Corona - lønkompensation";
                case "0172":
                    return "Corona - faste udgifter - kompensation";
                case "0173":
                    return "Corona - arrangørkompensation ";
                case "0174":
                    return "Corona - førtidig udbetaling af feriemidler";
                case "0175":
                    return "Corona - andre hjælpepakker, der ikke er selvstændig nævnt ";
                case "0176":
                    return "Corona - førtidig restudbetaling af feriemidler";
                case "0177":
                    return "Ordinær udbetaling fra Lønmodtagernes Feriemidler";
                default:
                    return "Ukendt indkomstart: " + angivelsesTekst;
            }
        }
        private string GetIndkomstTypeFromCode(string indkomsttype)
        {
            switch (indkomsttype)
            {
                case "0":
                    return "Løn";
                case "1":
                    return "";
                case "3":
                    return "Grønlandsk indkomst";
                case "4":
                    return "Anden personlig indkomst/ej lønansat";
                case "5":
                    return "B-indkomst";
                case "6":
                    return "Kontanthjælp";
                case "7":
                    return "Sygedagpenge";
                case "8":
                    return "§ 48 E og F forskerordningen";
                case "9":
                    return "Skattefri løn i ansættelsesforhold";
                case "10":
                    return "Udbetaling af løn Lønmodtagernes Garantifond";
                case "11":
                    return "Udlodning af løn fra opgjort bo";
                case "20":
                    return "Udbetaling fra AFU til lønmodtagere";
                case "24":
                    return "Løn uden lønindeholdelse";
                case "26":
                    return "A-indkomst mv. fra aldersopsparing";
                case "28":
                    return "A-indkomst fra Lønmodtagernes Feriefond";
                case "29":
                    return "Udbetaling af feriepenge m.m. fra FGO";
                default:
                    return "Ukendt indkomsttype: " + indkomsttype;

            }

        }
    }
}