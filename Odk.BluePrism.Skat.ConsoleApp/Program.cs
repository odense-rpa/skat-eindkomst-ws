using Odk.BluePrism.Skat.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
            // Load .env file from project root
            DotNetEnv.Env.Load();

#if DEBUG
            RunDemo();
#else
            RunProduction(args[0]);
#endif
            Console.ReadLine();
        }

        private static string GetEnvOrDefault(string key, string defaultValue = "")
        {
            return Environment.GetEnvironmentVariable(key) ?? defaultValue;
        }

        private static List<string> GetDemoSSNs()
        {
            var ssns = new List<string>();
            for (int i = 1; i <= 10; i++)
            {
                var ssn = GetEnvOrDefault($"DEMO_SSN_{i}");
                if (!string.IsNullOrWhiteSpace(ssn))
                {
                    ssns.Add(ssn);
                }
            }
            return ssns;
        }

        private static ServiceConfig GetDemoConfig()
        {
            return new ServiceConfig
            {
                DNSIdentity = GetEnvOrDefault("DEMO_DNS_IDENTITY", "SKAT OIO Gateway Test"),
                AuthenticationCertificateName = GetEnvOrDefault("DEMO_AUTH_CERT_NAME", "OIO Gateway Klient 3 Test"),
                SigningCertificateName = GetEnvOrDefault("DEMO_SIGNING_CERT_NAME", "SKAT OIO Gateway Test"),
                SENummer = GetEnvOrDefault("DEMO_SE_NUMMER", "19552101"),
                AbonnementTypeKode = GetEnvOrDefault("DEMO_ABONNEMENT_TYPE_KODE", "3153"),
                AbonnentTypeKode = GetEnvOrDefault("DEMO_ABONNENT_TYPE_KODE", "0750"),
                AdgangFormaalTypeKode = GetEnvOrDefault("DEMO_ADGANG_FORMAAL_TYPE_KODE", "171")
            };
        }

        private static ServiceConfig GetProdConfig()
        {
            return new ServiceConfig
            {
                DNSIdentity = GetEnvOrDefault("PROD_DNS_IDENTITY"),
                AuthenticationCertificateName = GetEnvOrDefault("PROD_AUTH_CERT_NAME"),
                SigningCertificateName = GetEnvOrDefault("PROD_SIGNING_CERT_NAME"),
                SENummer = GetEnvOrDefault("PROD_SE_NUMMER", "00000000"),
                AbonnementTypeKode = GetEnvOrDefault("PROD_ABONNEMENT_TYPE_KODE", "0000"),
                AbonnentTypeKode = GetEnvOrDefault("PROD_ABONNENT_TYPE_KODE", "0000"),
                AdgangFormaalTypeKode = GetEnvOrDefault("PROD_ADGANG_FORMAAL_TYPE_KODE", "000")
            };
        }

        private static void RunProduction(string ssn)
        {
            if (string.IsNullOrEmpty(ssn))
                throw new ArgumentNullException("ssn");

            EIndkomst e = new EIndkomst(new Spy(), GetProdConfig());

            string user = GetEnvOrDefault("PROD_USER", "SystemName");
            var ident = "XXXXXXXX" + DateTime.Now.ToString("yyyyMMdd_HHmmss.FFFF");

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
            var demoSSNs = GetDemoSSNs();

            if (!demoSSNs.Any())
            {
                Trace.TraceWarning("No demo SSNs found in .env file. Using defaults or add DEMO_SSN_1, DEMO_SSN_2, etc.");
            }

            EIndkomst e = new EIndkomst(new Spy(), GetDemoConfig());

            string ssn = demoSSNs.Any() ? demoSSNs[0] : "3004861026";
            string user = GetEnvOrDefault("DEMO_USER", "SystemNameTest");

            var ident = "TEST XXXXXX " + DateTime.Now.ToString("yyyyMMdd_HHmmss.FFFF");

            Trace.WriteLine("Kalder eindkomst IndkomstOplysningPersonHent TEST");
            Trace.WriteLine($"Bruger: {user}, ssn: {ssn}, ID: {ident}");
            if (demoSSNs.Any())
            {
                Trace.WriteLine($"Available test SSNs: {string.Join(", ", demoSSNs)}");
            }

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
