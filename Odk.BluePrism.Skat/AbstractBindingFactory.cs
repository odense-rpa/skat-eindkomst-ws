//#define USE_CONFIG  // Enable this flag only if configuration fra app.config is needed.//

using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Odk.BluePrism.Skat
{
    internal abstract class AbstractBindingFactory
    {
        public CustomBinding Binding { get; private set; }

        public EndpointAddress Endpoint { get; private set; }

        protected virtual CustomBinding CreateBinding()
        {
            /*
               This binding-sucker works:
           */
            var basicBinding = new BasicHttpBinding();
            basicBinding.Security.Mode = BasicHttpSecurityMode.TransportWithMessageCredential;
            basicBinding.MaxReceivedMessageSize = 10240000;                                     // always set to pretty large size as the response from eIndkomst has a lot of soap and data

            // create new instance of messagesecurity element
            var asyncSecurityBindingElement =
                (AsymmetricSecurityBindingElement)SecurityBindingElement.CreateMutualCertificateBindingElement(
                    MessageSecurityVersion.WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10
                );
            asyncSecurityBindingElement.AllowSerializedSigningTokenOnReply = true;              // MUST set to true
            asyncSecurityBindingElement.IncludeTimestamp = true;
            asyncSecurityBindingElement.SecurityHeaderLayout = SecurityHeaderLayout.Lax;        // MUST set to Lax

            var custom = new CustomBinding(basicBinding);
            custom.Elements.RemoveAt(0);                                                        // REMOVES the default transportsecuritybinding element from the element stack
            custom.Elements.Insert(0, asyncSecurityBindingElement);                             // .. and adds the new messagesecurity element as a replacement


            return custom;
        }

        protected abstract EndpointAddress CreateEndpointAddress(string dnsident);

        public void Setup(string dnsidentity)
        {
            Binding = CreateBinding();
            Endpoint = CreateEndpointAddress(dnsidentity);
        }
    }
}