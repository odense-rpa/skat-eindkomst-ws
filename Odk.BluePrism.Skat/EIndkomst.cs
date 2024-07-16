//#define USE_CONFIG      // Enable this flag only if configuration fra app.config is needed.//

using dk.skat.eindkomst;
using Odk.BluePrism.Skat.Parsers;
using Odk.BluePrism.Skat.Utils;
using System;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;

namespace Odk.BluePrism.Skat
{
    public class EIndkomst
    {

        private static ILog log;
        private readonly IeIndkomstParser responseParser;
        private bool getBasisMonth;

        public ServiceConfig Config { get; private set; }

        protected EIndkomst(ILog logger)
        {
            Config = StandardConfig();
            log = logger;
            responseParser = new eIndkomstParser(log);
        }

        protected EIndkomst()
        {
            Config = StandardConfig();
            responseParser = new eIndkomstParser();
        }

        public EIndkomst(ServiceConfig config)
        {
            Config = config;
            responseParser = new eIndkomstParser(log);
        }

        public EIndkomst(ILog logger, ServiceConfig config)
        {
            Config = config;
            log = logger;
            responseParser = new eIndkomstParser(log);
        }

        private ServiceConfig StandardConfig()
        {
#if DEBUG
            return new ServiceConfig
            {
                DNSIdentity = "xxx",
                AuthenticationCertificateName = "xxx",          // OCES3 Organisations Certifikat
                SigningCertificateName = "xxx",                 // Signeringscertifikat
                SENummer = "00000000",                          // Kommunes SE-nummer
                AbonnementTypeKode = "0000",                    // xxx ydelser
                AbonnentTypeKode = "0000",                      // Kommune
                AdgangFormaalTypeKode = "000"                   // xxx 
            };
#else
            return new ServiceConfig
            {
                DNSIdentity = "xxx",
                AuthenticationCertificateName = "xxx",          // OCES3 Organisations Certifikat
                SigningCertificateName = "xxx",                 // Signeringscertifikat
                SENummer = "00000000",                          // Kommunes SE-nummer
                AbonnementTypeKode = "0000",                    // xxx ydelser
                AbonnentTypeKode = "0000",                      // Kommune
                AdgangFormaalTypeKode = "000"                   // xxx
            };
#endif
        }


        private void LogInfo(string txt)
        {
            if (log != null)
            {
                log.Info(txt);
            }
        }

        public DataTable IndkomstOplysningPersonHent(string ssn, string workerid, DateTime startdate, DateTime enddate, string id, bool getbasismonth = false)
        {
            LogInfo("Current date period: " + startdate.ToShortDateString() + " " + enddate.ToShortDateString());
            return responseParser.ParseResultToDataTable(EIndkomstPersonHentKlient(ssn, workerid, startdate, enddate, id, getbasismonth));
        }

        public IndkomstOplysningPersonHent_OType EIndkomstPersonHentKlient(string ssn, string workerid, DateTime startdate, DateTime enddate, string id, bool getbasismonth = false)
        {
            getBasisMonth = getbasismonth;

            if (string.IsNullOrEmpty(ssn))
            {
                throw new ArgumentException($"'{nameof(ssn)}' cannot be null or empty.", nameof(ssn));
            }

            if (string.IsNullOrEmpty(workerid))
            {
                throw new ArgumentException($"'{nameof(workerid)}' cannot be null or empty.", nameof(workerid));
            }

            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException($"'{nameof(id)}' cannot be null or empty.", nameof(id));
            }

            return GetClientResponse(ssn, workerid, startdate, enddate, id);
        }

        private IndkomstOplysningPersonHent_OType GetClientResponse(string ssn, string workerid, DateTime startdate, DateTime enddate, string id)
        {
            IndkomstOplysningPersonHentServicePortTypeClient client;

#if USE_CONFIG

            var client = new IndkomstOplysningPersonHentServicePortTypeClient("client");
#else

#if DEBUG
            var bindingfactory = new DevelopmentBindingFactory();
           
            var sdbg = new System.ServiceModel.Description.ServiceDebugBehavior();
            sdbg.IncludeExceptionDetailInFaults = true;
#else
            var bindingfactory = new ProductionBindingFactory();
#endif
            bindingfactory.Setup(Config.DNSIdentity);
            client = new IndkomstOplysningPersonHentServicePortTypeClient(bindingfactory.Binding, bindingfactory.Endpoint);

            // CLIENT CERT:
            // Certificate embedded in WSSE-security header
            client.ClientCredentials.ClientCertificate.SetCertificate(
                StoreLocation.CurrentUser,
                StoreName.My,
                X509FindType.FindBySubjectName,
                Config.AuthenticationCertificateName 
            );

            // SERVER CERT: Skatteforvaltningen - SKAT OIO Gateway public certificate
            // - used to decrypt the response from Eindkomst service
            // TODO: refactor
            var servicecert = client.ClientCredentials.ServiceCertificate;
            servicecert.SetDefaultCertificate(
                StoreLocation.LocalMachine,
                StoreName.My,
                X509FindType.FindBySubjectName,
                Config.SigningCertificateName
            );

#endif
            var personHentType = new IndkomstOplysningPersonHent_IType();
            personHentType.HovedOplysninger = new HovedOplysningerType
            {
                TransaktionIdentifikator = id,                                  // TODO: ID is a reference to the service caller/datetime 
                TransaktionTid = DateTime.Now,
                TransaktionTidSpecified = true
            };

            personHentType.IndkomstOplysningPersonInddata = new IndkomstOplysningPersonHent_ITypeIndkomstOplysningPersonInddata
            {
                AbonnentStruktur = new AbonnentStrukturType
                {

                    AbonnentVirksomhedStruktur = new AbonnentVirksomhedStrukturType
                    {

                        AbonnentVirksomhed = new AbonnentVirksomhedStrukturTypeAbonnentVirksomhed
                        {
                            VirksomhedSENummerIdentifikator = Config.SENummer   // Skatteforvaltningens SE number used in DEMO
                        }
                    },
                    IndkomstOplysningAdgangMedarbejderIdentifikator = workerid
                },
                AbonnentAdgangStruktur = new AbonnentAdgangStrukturType
                {
                    AbonnementTypeKode = Config.AbonnementTypeKode,
                    AbonnentTypeKode = Config.AbonnentTypeKode,
                    AdgangFormaalTypeKode = Config.AdgangFormaalTypeKode,
                },
                IndkomstOplysningValg = new IndkomstOplysningPersonHent_ITypeIndkomstOplysningPersonInddataIndkomstOplysningValg
                {
                    Item = new IndkomstOplysningPersonHent_ITypeIndkomstOplysningPersonInddataIndkomstOplysningValgIndkomstPersonSamling
                    {
                        PersonIndkomstSoegeStruktur = new PersonIndkomstSoegeStrukturType[] {
                            GetSøgeStruktur(ssn, startdate, enddate, getBasisMonth)
                        }
                    }
                }

            };

            try
            {

                return client.getIndkomstOplysningPersonHent(personHentType);
            }
            catch (FaultException fe)
            {
                LogInfo(fe.Message);
                throw fe;
            }
            catch (Exception e)
            {
                LogInfo(e.Message);
                LogInfo(e.InnerException?.Message);
                throw e;
            }
            finally
            {
                client.Close();
            }
        }

        private static PersonIndkomstSoegeStrukturType GetSøgeStruktur(string ssn, DateTime startdate, DateTime enddate, bool getbasismonth)
        {
            
            var s = new PersonIndkomstSoegeStrukturType
            {
                PersonCivilRegistrationIdentifier = ssn

            };

            if (!getbasismonth)
            {
                s.SoegePeriodeLukketStruktur = new SoegePeriodeLukketStrukturType
                {
                    DateInterval = new DateIntervalType
                    {
                        Items = new object[] {
                                            startdate,
                                            enddate,
                                        },
                        ItemsElementName = new ItemsChoiceType[] {
                                            ItemsChoiceType.StartDate,
                                            ItemsChoiceType.EndDate
                                        }
                    }
                };
            }
            else
            {
                s.SoegeAarMaanedLukketStruktur = new SoegeAarMaanedLukketStrukturType
                {
                    SoegeAarMaanedFraKode = Dates.GetBasisMonthFromDate(startdate),
                    SoegeAarMaanedTilKode = Dates.GetBasisMonthFromDate(enddate)
                };
            }

            return s;
        }
    }
}