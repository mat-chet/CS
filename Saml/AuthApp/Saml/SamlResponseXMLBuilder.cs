using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Saml
{
    class SamlResponseXMLBuilder
    {
		

        public string idAssertion = "1";

		public SamlResponseXMLBuilder SetIdAssertion(string id)
        {
			this.idAssertion = id;
			return this;

		}

        public string issue_instant_Assertion = "1";

		public SamlResponseXMLBuilder SetIssueInstantAssertion(string IssueInstantAssertion)
		{
			this.issue_instant_Assertion = IssueInstantAssertion;
			return this;

		}

		public string notOnAfter = "1";
		public SamlResponseXMLBuilder SetNotOnAfter(string notOnAfter)
		{
			this.notOnAfter = notOnAfter;
			return this;

		}

		public string idResponse = "1";
		public SamlResponseXMLBuilder SetIdResponse(string id)
		{
			this.idResponse = id;
			return this;

		}

		public string issue_instant_Response = "1";
		public SamlResponseXMLBuilder SetIssueInstantResponse(string issue_instant_Response)
		{
			this.issue_instant_Response = issue_instant_Response;
			return this;

		}

		public string inResponseTo = "1";
		public SamlResponseXMLBuilder SetInResponseTo(string inResponseTo)
		{
			this.inResponseTo = inResponseTo;
			return this;

		}

		public string destination = "1";
		public SamlResponseXMLBuilder SetDestination(string destination)
		{
			this.destination = destination;
			return this;

		}

		public string issuer = "1";
		public SamlResponseXMLBuilder SetIssuer(string issuer)
		{
			this.issuer = issuer;
			return this;

		}

		public string nameID = "1";
		public SamlResponseXMLBuilder SetNameID(string nameID)
		{
			this.nameID = nameID;
			return this;

		}

		public string notBefore = "1";
		public SamlResponseXMLBuilder SetNotBefore(string notBefore)
		{
			this.notBefore = notBefore;
			return this;

		}

		public string audience = "1";
		public SamlResponseXMLBuilder SetAudience(string audience)
		{
			this.audience = audience;
			return this;

		}

		public string authnInstant = "1";
		public SamlResponseXMLBuilder SetAuthnInstant(string authnInstant)
		{
			this.authnInstant = authnInstant;
			return this;

		}

		public string sessionNotOnOrAfter = "1";
		public SamlResponseXMLBuilder SetSessionNotOnOrAfter(string sessionNotOnOrAfter)
		{
			this.sessionNotOnOrAfter = sessionNotOnOrAfter;
			return this;

		}

		public string sessionIndex = "1";
		public SamlResponseXMLBuilder SetSessionIndex(string sessionIndex)
		{
			this.sessionIndex = sessionIndex;
			return this;

		}




		public XmlDocument Build()
        {
			using (StringWriter sw = new StringWriter())
			{
				XmlWriterSettings xws = new XmlWriterSettings();
				xws.OmitXmlDeclaration = true;

				using (XmlWriter xw = XmlWriter.Create(sw, xws))
				{
					//
					xw.WriteStartElement("samlp", "Response", "urn:oasis:names:tc:SAML:2.0:protocol");
					xw.WriteAttributeString("xmlns", "saml",null, "urn:oasis:names:tc:SAML:2.0:assertion");
					xw.WriteAttributeString("ID", idResponse);
					xw.WriteAttributeString("Version", "2.0");
					xw.WriteAttributeString("IssueInstant", issue_instant_Response);
					xw.WriteAttributeString("InResponseTo", inResponseTo);
					xw.WriteAttributeString("Destination", destination);

					//
					xw.WriteStartElement("saml", "Issuer",null);
					xw.WriteString(issuer);
					xw.WriteEndElement();
					//

					//
					xw.WriteStartElement("saml", "Status", null);
					xw.WriteStartElement("saml", "StatusCode", null);
					xw.WriteAttributeString("Value", "urn:oasis:names:tc:SAML:2.0:status:Success");
					xw.WriteEndElement();
					xw.WriteEndElement();
					//

					//
					xw.WriteStartElement("saml", "Assertion", null);
					xw.WriteAttributeString("xmlns", "saml", null, "urn:oasis:names:tc:SAML:2.0:assertion");
					xw.WriteAttributeString("Version", "2.0");
					xw.WriteAttributeString("ID", idAssertion);
					xw.WriteAttributeString("IssueInstant", issue_instant_Assertion);
					

					
					//
					xw.WriteStartElement("saml", "Issuer", null);
					xw.WriteString(issuer);
					xw.WriteEndElement();
					//


					//
					xw.WriteStartElement("saml", "Subject", null);
					xw.WriteStartElement("saml", "NameID", null);
					xw.WriteAttributeString("Format", "urn:oasis:names:tc:SAML:2.0:nameid-format:unspecified");
					xw.WriteString(nameID);
					xw.WriteEndElement();

					xw.WriteStartElement("saml", "SubjectConfirmation", null);
					xw.WriteAttributeString("Method", "urn:oasis:names:tc:SAML:2.0:cm:bearer");

					xw.WriteStartElement("saml", "SubjectConfirmationData", null);
					xw.WriteAttributeString("NotOnOrAfter", notOnAfter);
					xw.WriteAttributeString("Recipient", destination);
					xw.WriteAttributeString("InResponseTo", inResponseTo);
					xw.WriteEndElement();

					xw.WriteEndElement();

					xw.WriteEndElement();//end subject
					//


					//
					xw.WriteStartElement("saml", "Conditions", null);
					xw.WriteAttributeString("NotBefore", issue_instant_Assertion);
					xw.WriteAttributeString("NotOnOrAfter", notOnAfter);


					xw.WriteStartElement("saml", "AudienceRestriction", null);
					xw.WriteStartElement("saml", "Audience", null);
					xw.WriteString(audience);
					xw.WriteEndElement();
					xw.WriteEndElement();

					xw.WriteEndElement();//end conditions
					//
					

					//
					xw.WriteStartElement("saml", "AuthnStatement", null);
					xw.WriteAttributeString("AuthnInstant", issue_instant_Assertion);
					xw.WriteAttributeString("SessionNotOnOrAfter", sessionNotOnOrAfter);
					xw.WriteAttributeString("SessionIndex", sessionIndex);


					xw.WriteStartElement("saml", "AuthnContext", null);
					xw.WriteStartElement("saml", "AuthnContextClassRef", null);
					xw.WriteString("urn:oasis:names:tc:SAML:2.0:ac:classes:PasswordProtectedTransport");
					xw.WriteEndElement();
					xw.WriteEndElement();

					xw.WriteEndElement();//end AuthnStatement
					//


					xw.WriteEndElement();
					//


					xw.WriteEndElement();

				}

				XmlDocument document = new XmlDocument();
				document.LoadXml(sw.ToString());
				return document;
				

			}
		}
    }
}
