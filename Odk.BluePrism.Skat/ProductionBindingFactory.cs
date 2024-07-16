//#define USE_CONFIG  // Enable this flag only if configuration fra app.config is needed.//

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Odk.BluePrism.Skat
{
    internal class ProductionBindingFactory : AbstractBindingFactory
    {
        protected override EndpointAddress CreateEndpointAddress(string dnsidentity)
        {
            return new EndpointAddress(
                    new Uri("https://services.extranet.skat.dk/vericert/services/IndkomstOplysningPersonHentV2ServicePort"),    // NOTE V2 
                    EndpointIdentity.CreateDnsIdentity(dnsidentity),
                    new AddressHeader[0]);
        }
    }
}