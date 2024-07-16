using Odk.BluePrism.Skat.Utils;
using System;
using System.Diagnostics;

namespace Odk.BluePrism.Skat.ConsoleApp
{

    class Spy : ILog
    {
        public void Error(string message)
        {
            Debug.WriteLine($"ERROR: {message}");
        }

        public void Info(string message)
        {
            Debug.WriteLine(message);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            RunDemo();
#else
            RunProduction(args[0]);
#endif
            Console.ReadLine();
        }

        private static void RunProduction(string ssn)
        {
            if (string.IsNullOrEmpty(ssn))
                throw new ArgumentNullException("ssn");

            // new EIndkomst(new Spy());   use Spy only as an option for debugging (and output in console only shown in debug mode).
            EIndkomst e = new EIndkomst(new Spy(), new ServiceConfig
            {
                DNSIdentity = "",
                AuthenticationCertificateName = "",                         // OCES3 Organisations Certifikat
                SigningCertificateName = "",                                // Signeringscertifikat
                SENummer = "00000000",                                      // Kommunes SE-nummer
                AbonnementTypeKode = "0000",                                // xxx
                AbonnentTypeKode = "0000",                                  // Kommune
                AdgangFormaalTypeKode = "000"                               // xxx
            });

            
            //string ssn = Environment.GetEnvironmentVariable("skatssn");                                            // set SSN here
            string user = "SystemName";

            var ident = "XXXXXXXX" + DateTime.Now.ToString("yyyyMMdd_hhmmss.FFFF");

            Trace.WriteLine("Kalder eindkomst IndkomstOplysningPersonHent PROD");
            Trace.WriteLine($"Bruger: {user}, ssn: {ssn}, ID: {ident}");
            bool basismonth = false;
            Trace.WriteLine($"Basismonth ?: {basismonth}");
            try
            {
                var datatable = e.IndkomstOplysningPersonHent(ssn, user, DateTime.Parse("2023.01.01"), DateTime.Parse("2023.01.31"), ident, basismonth);
                Trace.WriteLine("Done deal...");
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }
        }

        private static void RunDemo()
        {
            var demossns = new[] {                                      // SSNs for testing in the demo env.
               ""
            };

            var newssns = new[]
            {
                ""
            };

            // new EIndkomst(new Spy());   use Spy only as an option for debugging (and output in console only shown in debug mode).
            EIndkomst e = new EIndkomst(new Spy(), new ServiceConfig
            {
                DNSIdentity = "",
                AuthenticationCertificateName = "",                             // OCES3 Organisations Certifikat
                SigningCertificateName = "",                                    // Signeringscertifikat
                SENummer = "00000000",                                          // Kommunes SE-nummer
                AbonnementTypeKode = "0000",                                    // 
                AbonnentTypeKode = "0000",                                      // Kommune
                AdgangFormaalTypeKode = "000"                                   // 
            });                                                                 

            string ssn = newssns[0];                                            // set SSN here
            string user = "SystemNameTest";


            var ident = "TEST XXXXXX " + DateTime.Now.ToString("yyyyMMdd_hhmmss.FFFF");

            Trace.WriteLine("Kalder eindkomst IndkomstOplysningPersonHent TEST");
            Trace.WriteLine($"Bruger: {user}, ssn: {ssn}, ID: {ident}");
            try
            {
                var datatable = e.IndkomstOplysningPersonHent(ssn, user, DateTime.Parse("2023.01.01"), DateTime.Parse("2023.12.30"), ident);
                Trace.WriteLine("Done deal...");
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }
        }
    }
}
