using Saml;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace AuthApp.Controllers
{
    public class ResponseTool
    {
        public static XmlDocument CreateResponse(SamlRequestReader req)
        {
            //SamlRequestReader req = new SamlRequestReader(request);
            XmlDocument response = new SamlResponseXMLBuilder()
                .SetInResponseTo(req.GetAuthnRequestID())
                .SetDestination(req.GetAssertionConsumerServiceURL())
                .SetIssuer(req.GetIssuer())
                .SetNameID(req.GetNameID())
                .SetAudience(req.GetIssuer())
                .SetNotOnAfter(DateTime.Now.AddDays(365).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"))
                .SetIdResponse("_" + System.Guid.NewGuid().ToString())
                .SetIdAssertion("_" + System.Guid.NewGuid().ToString())
                .SetNotBefore(DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"))
                .SetSessionIndex("_samlong_")
                .SetIssueInstantAssertion(DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"))
                .SetIssueInstantResponse(DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"))
                .SetSessionNotOnOrAfter(DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"))
                .Build();


            return response;
        }

        public static XmlDocument SignXml(string XMLtext, string SubjectName, string privateKey)
        {
            if (null == XMLtext)
                throw new ArgumentNullException("XMLtext");
            if (null == SubjectName)
                throw new ArgumentNullException("SubjectName");

            // Load the certificate from the certificate store.
            X509Certificate2 cert = new X509Certificate2(StringToByteArray(SubjectName));

            // Create a new XML document.
            XmlDocument doc = new XmlDocument();

            // Format the document to ignore white spaces.
            doc.PreserveWhitespace = false;

            // Load the passed XML file using it's name.
            doc.LoadXml(XMLtext);

            // Create a SignedXml object.
            SignedXml signedXml = new SignedXml(doc);

            RSA rsa = new RSACng();
            rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);
            // Add the key to the SignedXml document. 
            signedXml.SigningKey = rsa;

            // Create a reference to be signed.
            Reference reference = new Reference();
            reference.Uri = "";

            // Add an enveloped transformation to the reference.
            XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
            reference.AddTransform(env);

            // Add the reference to the SignedXml object.
            signedXml.AddReference(reference);

            // Create a new KeyInfo object.
            KeyInfo keyInfo = new KeyInfo();

            // Load the certificate into a KeyInfoX509Data object
            // and add it to the KeyInfo object.
            keyInfo.AddClause(new KeyInfoX509Data(cert));

            // Add the KeyInfo object to the SignedXml object.
            signedXml.KeyInfo = keyInfo;

            // Compute the signature.
            signedXml.ComputeSignature();

            // Get the XML representation of the signature and save
            // it to an XmlElement object.
            XmlElement xmlDigitalSignature = signedXml.GetXml();

            // Append the element to the XML document.
            doc.DocumentElement.AppendChild(doc.ImportNode(xmlDigitalSignature, true));

            if (doc.FirstChild is XmlDeclaration)
            {
                doc.RemoveChild(doc.FirstChild);
            }

            // Save the signed XML document to a file specified
            // using the passed string.
            return doc;
        }

        public static byte[] StringToByteArray(string st)
        {
            byte[] bytes = new byte[st.Length];
            for (int i = 0; i < st.Length; i++)
            {
                bytes[i] = (byte)st[i];
            }
            return bytes;
        }
    }
}
