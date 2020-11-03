using Saml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Saml
{
    public class SamlRequestReader
    {
        XmlDocument request;
        XmlNamespaceManager manager;

        public SamlRequestReader(XmlDocument xml)
        {
            request = new XmlDocument();
            request.LoadXml(xml.OuterXml);
            manager = new XmlNamespaceManager(request.NameTable);
            manager.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion");
            manager.AddNamespace("samlp", "urn:oasis:names:tc:SAML:2.0:protocol");

        }



        public string GetIssuer()
        {
            XmlNode node = request.SelectSingleNode("/samlp:AuthnRequest/saml:Issuer", manager);
            return node.InnerText;
        }

        public string GetNameID()
        {
            XmlNode node = request.SelectSingleNode("/samlp:AuthnRequest/saml:Subject/saml:NameID", manager);
            return node.InnerText;
        }
        public string GetAuthnRequestID()
        {
            XmlNode xmlNode = request.DocumentElement;

            string ID = xmlNode.Attributes.GetNamedItem("ID").InnerText;
            return ID;
        }

        public string GetIssueInstant()
        {

            XmlNode xmlNode = request.DocumentElement;

            string instant = xmlNode.Attributes.GetNamedItem("IssueInstant").InnerText;
            return instant;
        }

        public string GetAssertionConsumerServiceURL()
        {
            XmlNode xmlNode = request.DocumentElement;

            string url = xmlNode.Attributes.GetNamedItem("AssertionConsumerServiceURL").InnerText;
            return url;
        }

        public string GetAuthnContextClassRef()
        {
            XmlNode node = request.SelectSingleNode("/samlp:AuthnRequest/samlp:RequestedAuthnContext/saml:AuthnContextClassRef", manager);
            return node.InnerText;
        }
    }
}

