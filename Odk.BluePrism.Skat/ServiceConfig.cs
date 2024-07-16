//#define USE_CONFIG  // Enable this flag only if configuration fra app.config is needed.//

namespace Odk.BluePrism.Skat
{
    public struct ServiceConfig
    {
        /// <summary>
        /// DK: SKal sættes til signeringscertifikatets CN værdi i Emne (f.eks: 'SKAT OIO Gateway Prod').
        /// EN: TODO
        /// </summary>
        public string DNSIdentity { get; set; }

        /// <summary>
        /// DK: Navnet på authentificerings-certifikatet (clientcertificate) som er placeret i CERTSTORE:/Aktuel Bruger/Personlig/Certifikater:
        /// EN: TODO
        /// </summary>
        public string AuthenticationCertificateName { get; set; }


        /// <summary>
        /// DK: Navnet på signerings-certifikatet (servercertificate) som er placeret i CERTSTORE:/Computer/Personlig/Certifikater:
        /// EN: TODO
        /// </summary>
        public string SigningCertificateName { get; set; }

        /// <summary>
        /// DK: Klientens SE-nummer som angivet i aftalen med eIndkomst.
        /// EN: TODO
        /// </summary>
        public string SENummer { get; set; }

        /// <summary>
        /// DK: Klientens abonnenment type kode. (f.eks: "3153" ->  Indkomstafhængige ydelser). Udleveres af eIndkomst
        /// EN: TODO
        /// </summary>
        public string AbonnementTypeKode { get; set; }

        /// <summary>
        /// DK: Klientens abonnent type kode. . Udleveres af eIndkomst 
        /// EN: TODO
        /// </summary>
        public string AbonnentTypeKode { get; set; }

        /// <summary>
        /// DK: Angiver klientens formål med adgangen til eIndkomst (f.eks: "171" -> Aktiv beskæftigelsesindsats – Flekslønstilskud). Udleveres af eIndkomst
        /// EN: TODO 
        /// </summary>
        public string AdgangFormaalTypeKode { get; set; }
    }
}